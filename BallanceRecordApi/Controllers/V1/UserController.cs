using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Contracts.V1.Requests;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Extensions;
using BallanceRecordApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BallanceRecordApi.Controllers.V1
{
    public class UserController: Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        private readonly IUriService _uriService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserController(IIdentityService identityService, IEmailService emailService, IUriService uriService, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _identityService = identityService;
            _emailService = emailService;
            _uriService = uriService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiRoutes.Identity.GetSelf)]
        public async Task<IActionResult> Get()
        {
            var user = await _identityService.GetUserById(Guid.Parse(HttpContext.GetUserId()));
            
            return Ok(_mapper.Map<UserInfoResponse>(user));
        }
        
        [HttpGet(ApiRoutes.Identity.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid userId)
        {
            var user = await _identityService.GetUserById(userId);
            if (user is null)
                return NotFound();
            var response = new BriefUserInfoResponse
            {
                Username = user.UserName
            };
            return Ok(response);
        }
        
        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            
            var authResponse = await _identityService.RegisterAsync(request.Email, request.Password, request.Username);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = authResponse.Messages
                });
            }

            var userConfirmationUri = _uriService.GetUserConfirmationUri(
                authResponse.Messages.ToArray()[1],
                authResponse.Messages.ToArray()[2]
            );

            try
            {
                var emailContentPath = Path.Combine(_webHostEnvironment.WebRootPath, "EmailContent.html");
                var emailContent = await System.IO.File.ReadAllTextAsync(emailContentPath);
                emailContent = emailContent.Replace("{link}", userConfirmationUri.ToString());
                emailContent = emailContent.Replace("{username}", request.Username);
                // Console.WriteLine(request.Email);
                // Console.WriteLine(emailContent);
                await _emailService.SendAsync(request.Email, "BallanceMMO Verification Email", emailContent);

                return Accepted(new AuthSuccessResponse
                {
                    Username = authResponse.Username,
                    Token = authResponse.Token,
                    RefreshToken = authResponse.RefreshToken,
                    Messages = new[]
                    {
                        authResponse.Messages.FirstOrDefault(x => !string.IsNullOrEmpty(x))
                    }
                });
            }
            catch (IOException e)
            {
                Console.WriteLine("Email content not found.");
                Console.WriteLine(e);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthFailResponse
                {
                    Errors = new[]
                    {
                        authResponse.Messages.FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                        "Server has encountered a bug: Email content not found. Please contact admin.",
                        // locationUri.ToString()
                    }
                });
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                Console.WriteLine("Server cannot connect to mail service.");
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthFailResponse
                {
                    Errors = new[]
                    {
                        authResponse.Messages.FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                        "Server has encountered a bug: Server cannot connect to mail service. Please contact admin.",
                        // locationUri.ToString()
                    }
                });
            }
        }
        
        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = authResponse.Messages
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Username = authResponse.Username,
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
        
        [HttpPut(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.RefreshToken);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = authResponse.Messages
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Username = authResponse.Username,
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
        
        

        [HttpGet(ApiRoutes.Identity.Confirmation)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, string token)
        {
            var authResponse = await _identityService.ConfirmEmailAsync(userId, token);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = authResponse.Messages
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Username = authResponse.Username,
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
    }
}