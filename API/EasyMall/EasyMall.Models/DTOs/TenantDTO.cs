using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Models.DTOs
{
    public class TenantDTO : BaseDto
    {
        public string Name { get; set; }
        public TenantTypes Type { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
