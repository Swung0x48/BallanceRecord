using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BallanceRecordApi.Data;
using BallanceRecordApi.Domain;
using BallanceRecordApi.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BallanceRecordApi.Services
{
    public class IdentityService: IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtOptions _jwtOptions;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _dataContext;

        private enum EmailType
        {
            Register,
            Reset,
            EmailChange
        }
        
        public IdentityService(UserManager<IdentityUser> userManager, JwtOptions jwtOptions, TokenValidationParameters tokenValidationParameters, DataContext dataContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _tokenValidationParameters = tokenValidationParameters;
            _dataContext = dataContext;
            _roleManager = roleManager;
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password, string username)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                return new AuthenticationResult
                {
                    Messages = new []{"User with this email address already exists."}
                };
            }

            var newUserId = Guid.NewGuid();
            var newUser = new IdentityUser
            {
                Id = newUserId.ToString(),
                Email = email,
                UserName = username,
                EmailConfirmed = false
            };
            
            var userCreated = await _userManager.CreateAsync(newUser, password);
            
            if (!userCreated.Succeeded)
            {
                return new AuthenticationResult
                {
                    Messages = userCreated.Errors.Select(x => x.Description)
                };
            }

            // await _userManager.AddToRoleAsync(newUser, "Admin");
            await _userManager.AddToRoleAsync(newUser, "User");

            return new AuthenticationResult
            {
                Success = true,
                Messages = new []
                {
                    "Please check your inbox in order to validate your email.", 
                    newUser.Id, 
                    await _userManager.GenerateEmailConfirmationTokenAsync(newUser)
                }
            };
            // return await GenerateAuthenticationResultForUserAsync(newUser);
        }
        
        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            
            if (user is null)
            {
                return new AuthenticationResult
                {
                    Messages = new []{"User does not exist."}
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Messages = new []{"Username or password is not correct"}
                };
            }
            
            if (!user.EmailConfirmed)
            {
                return new AuthenticationResult
                {
                    Messages = new []{"Email not confirmed."}
                };
            }
            
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString()) is not null;
        }
        
        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
        {
            // var validatedToken = GetPrincipalFromToken(token);
            //
            // if (validatedToken is null)
            // {
            //     return new AuthenticationResult {Messages = new []{"Invalid Token"}};
            // }

            // var expiryDateUnix =
            //     long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            //
            // var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            //     .AddSeconds(expiryDateUnix);
            //
            // if (expiryDateTimeUtc > DateTime.UtcNow)
            // {
            //     return new AuthenticationResult {Messages = new []{"This token hasn't expired yet."}};
            // }

            // var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken is null)
            {
                return new AuthenticationResult{ Messages = new []{"This refresh token does not exist." }};
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryTime)
            {
                return new AuthenticationResult { Messages = new []{"This refresh token has expired." }};
            }
            
            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Messages = new[] {"This refresh token has been used." }};
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Messages = new []{"This refresh token has been invalidated." }};
            }

            // if (storedRefreshToken.JwtId != jti)
            // {
            //     return new AuthenticationResult { Messages = new[] {"This refresh token does not match this JWT."} };
            // }

            storedRefreshToken.Used = true;
            storedRefreshToken.Invalidated = true;
            _dataContext.RefreshTokens.Update(storedRefreshToken);
            await _dataContext.SaveChangesAsync();
            
            var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
            
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return new AuthenticationResult
                {
                    Messages = new []{ "User not found." }
                };
            }

            var identityResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!identityResult.Succeeded)
            {
                return new AuthenticationResult
                {
                    Messages = identityResult.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var changeResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!changeResult.Succeeded)
            {
                return new AuthenticationResult
                {
                    Messages = changeResult.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> ChangeEmailAsync(string email, string newEmail)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            // var context = _httpContextAccessor.HttpContext;
            // var baseUrl = $"{context.Request.Scheme}://{context.Request.Host.ToUriComponent()}";
            // TODO: Link Generator
            // var html = (await File.ReadAllTextAsync("Static/EmailContext.html", Encoding.UTF8))
                // .Replace("{{link}}", $"{baseUrl}/{ApiRoutes.Identity.Email}?");
            // TODO

            throw new NotImplementedException();
            //return await SendEmailAsync()
            //return await SendEmailAsync(user.Email, user.Id, EmailType.EmailChange, newEmail);
        }

        public async Task<IList<string>> GetRolesAsync(IdentityUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<AuthenticationResult> ResetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return new AuthenticationResult
            {
                Messages = new []{"A validation link has been sent to your email.", await _userManager.GeneratePasswordResetTokenAsync(user)}
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                return IsJwtWithValidSecurityAlgorithm(validatedToken) ? principal : null;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            if (!user.EmailConfirmed)
            {
                return new AuthenticationResult
                {
                    Messages = new[] {"Email has not been confirmed yet."}
                };
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if(role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtOptions.TokenLifetime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryTime = DateTime.Now.AddMonths(6)
            };

            await _dataContext.RefreshTokens.AddAsync(refreshToken);
            await _dataContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Username = user.UserName,
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
    }
}