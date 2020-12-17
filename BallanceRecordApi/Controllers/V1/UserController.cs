using System;
using System.Linq;
using System.Threading.Tasks;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Contracts.V1.Requests;
using BallanceRecordApi.Contracts.V1.Responses;
using BallanceRecordApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BallanceRecordApi.Controllers.V1
{
    public class UserController: Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        private readonly IUriService _uriService;
        public UserController(IIdentityService identityService, IEmailService emailService, IUriService uriService)
        {
            _identityService = identityService;
            _emailService = emailService;
            _uriService = uriService;
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
                return Unauthorized(new AuthFailResponse
                {
                    Errors = authResponse.Messages
                });
            }

            var rawHtml = await System.IO.File.ReadAllTextAsync("Static/EmailContent.html");
            // var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            // var locationUri = $"{baseUrl}/{ApiRoutes.Identity.Confirmation}" +
            //                   $"?userid={Uri.EscapeDataString(authResponse.Messages.ToArray()[1])}" +
            //                   $"&token={Uri.EscapeDataString(authResponse.Messages.ToArray()[2])}";

            var locationUri = _uriService.GetUserConfirmationUri(
                authResponse.Messages.ToArray()[1],
                authResponse.Messages.ToArray()[2]
                );
            var emailContent = rawHtml.Replace("{link}", locationUri.ToString());
            await _emailService.SendAsync(request.Email, "Ballance Register Confirmation Email", emailContent);
            
            return Unauthorized(new AuthFailResponse
            {
                Errors = new []
                {
                    authResponse.Messages.FirstOrDefault(x => !string.IsNullOrEmpty(x))
                }
            });
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
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPut(ApiRoutes.Identity.Email)]
        public async Task<IActionResult> ChangeEmail()
        {
            throw new NotImplementedException();
        }
    }
}