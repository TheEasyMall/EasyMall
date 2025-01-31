using EasyMall.Commons.Enums;
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
        public Status Status { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingMethod { get; set; }
        public double ShippingFee { get; set; }
    }
}
