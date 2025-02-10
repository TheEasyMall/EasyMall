using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Request
{
    public class ReviewRequest : BaseDto
    {
        public Ratings Rating { get; set; }
        public string? Comment { get; set; }
        public Guid? ProductId { get; set; }
    }
}
