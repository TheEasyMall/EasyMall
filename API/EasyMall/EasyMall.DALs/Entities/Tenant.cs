using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.DALs.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public TenantTypes Type { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<User>? Users { get; set; }
        public ICollection<Category>? Categories { get; set; }
    }
}
