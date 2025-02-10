using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Request
{
    public class OrderRequest : BaseDto
    {
        public Status Status { get; set; }
        public Address ProductAddress { get; set; }
        public string ShippingAddress { get; set; }
        public double ShippingFee { get; set; }

        public List<Guid>? CartIds { get; set; }
    }
}
