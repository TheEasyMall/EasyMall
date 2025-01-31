﻿using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.DALs.Entities
{
    public class Order : BaseEntity
    {
        public double TotalAmount { get; set; }
        public Status Status { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingMethod { get; set; }
        public double ShippingFee { get; set; }

        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        [ForeignKey("Cart")]
        public Guid? CartId { get; set; }
        public Cart? Cart { get; set; }
    }
}
