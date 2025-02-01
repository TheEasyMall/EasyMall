using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs
{
    public class ReviewDTO : BaseDto
    {
        public Ratings Rating { get; set; }
        public string? Comment { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ProductId { get; set; }
    }
}
