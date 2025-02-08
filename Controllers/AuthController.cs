using CommerceApp.DTOs.ApiUserDtos.Request;
using CommerceApp.DTOs.ApiUserDtos.Response;
using CommerceApp.Models.AuthModels;
using CommerceApp.Repositories.AuthRepository;
using CommerceApp.Repositories.EmailServiceRepository;
using CommerceApp.Templates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace CommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<ApiUser> _userManager;
        private readonly EmailService _emailService;

        public AuthController(IAuthRepository authRepository,
            UserManager<ApiUser> userManager,
            EmailService emailService)
        {
            _authRepository = authRepository;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Response_ApiUserRegisterDto>> Register
            ([FromBody] Request_ApiUserRegisterDto request_ApiUserRegisterDto)
        {
            var userDto = await _authRepository.Register(request_ApiUserRegisterDto);

            if(userDto.IsSuccess == false)
            {
                return BadRequest(new Response_ApiUserRegisterDto()
                {
                    IsSuccess = false,
                    Message = userDto.Message
                });
            }

            // Generate user confirmation token
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userDto.apiUser);

            //https

            var encodedCode = HttpUtility.UrlEncode(code);
            var callbackUrl = $"{Request.Scheme}://{Request.Host}{Url.Action("ConfirmEmail", "Auth", new { userId = userDto.apiUser.Id, code = encodedCode })}";
            var body = EmailTemplates.EmailLinkTemplate(callbackUrl);

            // Send email
            await _emailService.SendEmailAsync(userDto.apiUser.Email, "Confirm Your Email", body);

            try
            {
                return Ok(new Response_ApiUserRegisterDto()
                {
                    IsSuccess = true,
                    Message = new List<string> { "Registration successful.", "Please check your email to confirm your account." }
                });
            }
            catch (Exception ex)
            {
                return Ok(new Response_ApiUserRegisterDto()
                {
                    IsSuccess = true,
                    Message = new List<string>
                    {
                        "Registration successful.",
                        "However, we could not send the confirmation email. Please contact support."
                    }
                });
            }
        }

        [HttpPost]
        [Route("registerAdmin/(secretKey)")]
        public async Task<ActionResult<Response_ApiUserRegisterDto>> RegisterAdmin(
            [FromRoute] int secretKey, [FromBody] Request_ApiUserRegisterDto request)
        {
            var userDto = await _authRepository.RegisterAdmin(request, secretKey);
            if(userDto.IsSuccess == false)
            {
                return BadRequest(
                    new Response_ApiUserRegisterDto()
                    {
                        IsSuccess = false,
                        Message = userDto.Message
                    });
            }
            return Ok(new Response_ApiUserRegisterDto()
            {
                IsSuccess = true,
                Message = new List<string>
                {
                    "Admin user created successfully",
                    "Admin user don't need to email"
                }
            });

        }


        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<ActionResult<Response_ApiUserConfirmEmail>> ConfirmEmail(
            string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest(new Response_ApiUserConfirmEmail()
                {
                    IsSuccess = false,
                    Message = "Wrong email confirmation link"
                });
            } 

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new Response_ApiUserConfirmEmail()
                {
                    IsSuccess = false,
                    Message = "Wrong user ID provided"
                });
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            var status = result.Succeeded ? "Thank you for confirming email address"
                : "Your email address is not confirmed, please try again later";

                return Ok(new Response_ApiUserConfirmEmail()
                {
                    IsSuccess = true,
                    Message = status
                });

        }

    }
}
