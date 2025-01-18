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

        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}
