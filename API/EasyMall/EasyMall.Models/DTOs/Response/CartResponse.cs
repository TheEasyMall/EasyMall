using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Response
{
    public class CartResponse : BaseDto
    {
        public int Quantity { get; set; }
        public string Type { get; set; }
        public double TotalAmount { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid? VariantId { get; set; }
        public string VariantName { get; set; }

        public List<ProductResponse>? Products { get; set; }
    }
}
