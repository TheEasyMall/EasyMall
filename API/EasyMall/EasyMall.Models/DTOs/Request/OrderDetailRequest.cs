using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Request
{
    public class OrderDetailRequest : BaseDto
    {
        public int Quantity { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? ProductId { get; set; }
    }
}
