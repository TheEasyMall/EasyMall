using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Response
{
    public class CategoryResponse : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPresent { get; set; }

        public List<ProductResponse>? Products { get; set; }
    }
}
