using EasyMall.Commons.Enums;
using MayNghien.Infrastructure.Models;

namespace EasyMall.Models.DTOs.Response
{
    public class ProductResponse : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public Address Address { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
