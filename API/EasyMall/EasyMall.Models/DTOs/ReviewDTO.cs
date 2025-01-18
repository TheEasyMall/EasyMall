using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Models.DTOs
{
    public class ReviewDTO : BaseDto
    {
        public Ratings Rating { get; set; }
        public string Comment { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ProductId { get; set; }
    }
}
