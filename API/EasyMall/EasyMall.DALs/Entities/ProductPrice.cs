using MayNghien.Infrastructure.Models.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyMall.DALs.Entities
{
    public class ProductPrice : BaseEntity
    {
        public string? Type { get; set; }
        public double Price { get; set; }

        [ForeignKey("Product")]
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }

        public ICollection<Cart>? Carts { get; set; }
    }
}
