using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyMall.DALs.Entities
{
    public class Order : BaseEntity
    {
        public double TotalAmount { get; set; }
        public Status Status { get; set; }
        public Address ProductAddress { get; set; }
        public string ShippingAddress { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public double ShippingFee { get; set; }

        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
