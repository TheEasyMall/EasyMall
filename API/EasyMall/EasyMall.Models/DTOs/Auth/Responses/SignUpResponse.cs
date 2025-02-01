using EasyMall.Commons.Enums;

namespace EasyMall.Models.DTOs.Auth.Responses
{
    public class SignUpResponse
    {
        public string Email { get; set; }
        public TenantTypes Type { get; set; }
        public string Token { get; set; }
    }
}
