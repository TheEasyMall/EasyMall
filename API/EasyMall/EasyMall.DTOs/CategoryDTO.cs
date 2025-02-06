using MayNghien.Infrastructure.Models;

namespace EasyMall.DTO
{
    public class CategoryDTO : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPresent { get; set; }

        public List<ProductDTO>? Products { get; set; }
    }
}
