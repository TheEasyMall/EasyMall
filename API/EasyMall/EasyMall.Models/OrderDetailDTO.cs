using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.DTO
{
    public class OrderDetailDTO : BaseDto
    {
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
