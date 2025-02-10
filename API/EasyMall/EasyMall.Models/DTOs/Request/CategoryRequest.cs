using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Request
{
    public class CategoryRequest : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPresent { get; set; }
    }
}
