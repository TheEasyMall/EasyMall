using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs
{
    public class ProductDTO : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
