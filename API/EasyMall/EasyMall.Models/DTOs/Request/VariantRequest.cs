using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Request
{
    public class VariantRequest : BaseDto
    {
        public string? Type { get; set; }
        public double Price { get; set; }
        public Guid? ProductId { get; set; }
    }
}
