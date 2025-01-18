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
        public double Price { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey("Product")]
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
