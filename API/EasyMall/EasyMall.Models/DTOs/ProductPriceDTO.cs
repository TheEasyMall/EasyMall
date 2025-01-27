using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Models.DTOs
{
    public class ProductPriceDTO : BaseDto
    {
        public Guid? ProductId { get; set; }
        public double Price { get; set; }
        public string? Type { get; set; }
    }
}
