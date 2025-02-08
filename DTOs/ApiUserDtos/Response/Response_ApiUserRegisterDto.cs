using CommerceApp.Models.AuthModels;

namespace CommerceApp.DTOs.ApiUserDtos.Response
{
    public class Response_ApiUserRegisterDto
    {
        public bool IsSuccess { get; set; }
        public ApiUser ? apiUser { get; set; }
        public List<string> Message { get; set; } = new List<string>();
    }
}
