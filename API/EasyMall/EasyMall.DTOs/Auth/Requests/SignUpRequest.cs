using EasyMall.Commons.Enums;

namespace EasyMall.DTO.Auth.Requests
{
    public class SignUpRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public TenantTypes Type { get; set; }
    }
}
