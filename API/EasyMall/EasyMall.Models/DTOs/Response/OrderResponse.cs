using EasyMall.Commons.Enums;
using EasyMall.DTOs.DTOs;
using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Models.DTOs.Response
{
    public class OrderResponse : BaseDto
    {
        public double TotalAmount { get; set; }
        public Status Status { get; set; }
        public Address ProductAddress { get; set; }
        public string ShippingAddress { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public double ShippingFee { get; set; }
        
        public List<OrderDetailResponse>? OrderDetails { get; set; }
    }
}
