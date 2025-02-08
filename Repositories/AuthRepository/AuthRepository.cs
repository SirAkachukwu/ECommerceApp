using CommerceApp.Constants;
using CommerceApp.Data;
using CommerceApp.DTOs.ApiUserDtos.Request;
using CommerceApp.DTOs.ApiUserDtos.Response;
using CommerceApp.Models.AuthModels;
using Microsoft.AspNetCore.Identity;

namespace CommerceApp.Repositories.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public AuthRepository(UserManager<ApiUser> userManager, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<Response_ApiUserRegisterDto> Register(Request_ApiUserRegisterDto userDto, int secretKey)
        {

            if (secretKey != 12345)
            {
                return new Response_ApiUserRegisterDto()
                {
                    IsSuccess = false,
                    Message = new List<string>
                    {
                        "Wrong secret key"
                    }
                };
            }

            var user = new ApiUser()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.Lastname,
                Email = userDto.Email,
            };

            user.UserName = userDto.Email;
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Customer);

                return new Response_ApiUserRegisterDto()
                {
                    IsSuccess = true,
                    apiUser = user,
                };
            }

            List<String> errors = new List<String>();

            foreach(var error in result.Errors)
            {
                errors.Add(error.Description.ToString());
            }

            return new Response_ApiUserRegisterDto()
            {
                IsSuccess = true,
                Message = errors
            };
        }

        public async Task<Response_ApiUserRegisterDto> Register(Request_ApiUserRegisterDto userDto)
        {
            int defaultSecretKey = 12345;

            return await Register(userDto, defaultSecretKey);
        }

        public async Task<Response_ApiUserRegisterDto> RegisterAdmin(Request_ApiUserRegisterDto userDto, int secretKey)
        {
            if (secretKey != 12345)
            {
                return new Response_ApiUserRegisterDto()
                {
                    IsSuccess = false,
                    Message = new List<string> { "Wrong secret key" }
                };
            }

            var user = new ApiUser()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.Lastname,
                Email = userDto.Email,
            };

            user.UserName = userDto.Email;
            user.EmailConfirmed = false;

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Administrator);

                return new Response_ApiUserRegisterDto()
                {
                    IsSuccess = true,
                    apiUser = user,
                };
            }

            List<String> errors = new List<String>();

            foreach (var error in result.Errors)
            {
                errors.Add(error.Description.ToString());
            }

            return new Response_ApiUserRegisterDto()
            {
                IsSuccess = true,
                Message = errors
            };
        }
    }
}
