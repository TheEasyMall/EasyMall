using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;

namespace EasyMall.DTO
{
    public class ReviewDTO : BaseDto
    {
        public Ratings Rating { get; set; }
        public string? Comment { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
