using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models.Entity;

namespace EasyMall.DALs.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public TenantTypes Type { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<Category>? Categories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
