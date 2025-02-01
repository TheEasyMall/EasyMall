using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs
{
    public class CartDTO : BaseDto
    {
        public int Quantity { get; set; }
        public string Type { get; set; }
        public double TotalAmount { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductPriceId { get; set; }
    }
}
