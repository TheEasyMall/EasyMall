using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Models.DTOs
{
    public class OrderDTO : BaseDto
    {
        public double TotalAmount { get; set; }
        public Guid? UserId { get; set; }
    }
}
