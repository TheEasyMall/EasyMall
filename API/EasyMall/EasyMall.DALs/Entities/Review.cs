using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.DALs.Entities
{
    public class Review : BaseEntity
    {
        public Ratings Rating { get; set; }
        public string Comment { get; set; }

        [ForeignKey("User")]
        public Guid? UesrId { get; set; }
        public User? User { get; set; }

        [ForeignKey("Product")]
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
