using Microsoft.AspNetCore.Identity;

namespace CommerceApp.Models.AuthModels
{
    public class ApiUser : IdentityUser
    {
        public string ? FirstName { get; set; } 
        public string ? LastName { get; set; }
    }
}
