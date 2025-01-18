using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Models.DTOs
{
    public class UserDTO : BaseDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
        public Guid? TenantId { get; set; }
    }
}
