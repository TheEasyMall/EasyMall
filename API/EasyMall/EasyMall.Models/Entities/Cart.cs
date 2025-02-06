using MayNghien.Infrastructure.Models.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyMall.DALs.Entities
{
    public class Cart : BaseEntity
    {
        public int Quantity { get; set; }
        public string Type { get; set; }
        public double TotalAmount { get; set; }

        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        [ForeignKey("Product")]
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("ProductPrice")]
        public Guid? ProductPriceId { get; set; }
        public ProductPrice? ProductPrice { get; set; }
    }
}
