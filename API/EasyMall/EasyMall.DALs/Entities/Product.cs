using MayNghien.Infrastructure.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.DALs.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        //Chưa dùng
        //public int StockQuantity { get; set; }
        //public int ReservedQuantity { get; set; }
        //public int ReOrderLevel { get; set; }

        [ForeignKey("Category")]
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Cart>? Carts { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ProductPrice>? ProductPrices { get; set; }
    }
}
