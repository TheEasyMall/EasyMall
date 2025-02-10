using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Request
{
    public class CartRequest : BaseDto
    {
        public int Quantity { get; set; }
        public string Type { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductPriceId { get; set; }
    }
}
