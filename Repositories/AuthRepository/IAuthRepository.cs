using CommerceApp.DTOs.ApiUserDtos.Request;
using CommerceApp.DTOs.ApiUserDtos.Response;

namespace CommerceApp.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        Task<Response_ApiUserRegisterDto> Register(Request_ApiUserRegisterDto userDto);
        Task<Response_ApiUserRegisterDto> RegisterAdmin(Request_ApiUserRegisterDto userDto, int secretKey);
    }
}
