using MayNghien.Infrastructure.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.DALs.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }

        [ForeignKey("Order")]
        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }

        [ForeignKey("Product")]
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public Product? Product { get; set; }
    }
}
