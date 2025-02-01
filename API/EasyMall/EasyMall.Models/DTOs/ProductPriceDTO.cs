using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs
{
    public class ProductPriceDTO : BaseDto
    {
        public Guid? ProductId { get; set; }
        public double Price { get; set; }
        public string? Type { get; set; }
    }
}
