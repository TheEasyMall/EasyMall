﻿using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Models.DTOs
{
    public class CartDTO : BaseDto
    {
        public int Quantity { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ProductId { get; set; }
    }
}
