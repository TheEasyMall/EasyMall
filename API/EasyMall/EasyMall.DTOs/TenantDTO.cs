using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;

namespace EasyMall.DTO
{
    public class TenantDTO : BaseDto
    {
        public string Name { get; set; }
        public TenantTypes Type { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
