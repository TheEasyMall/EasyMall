using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs
{
    public class OrderDTO : BaseDto
    {
        public double TotalAmount { get; set; }
        public Status Status { get; set; }
        public string ShippingAddress { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public double ShippingFee { get; set; }
        public List<Guid>? CartIds { get; set; }
    }
}
