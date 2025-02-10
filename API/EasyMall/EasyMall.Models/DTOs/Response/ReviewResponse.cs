using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Response
{
    public class ReviewResponse : BaseDto
    {
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public Ratings Rating { get; set; }
        public string? Comment { get; set; }
    }
}
