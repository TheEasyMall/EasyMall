using AutoMapper;
using EasyMall.DALs.Entities;
using EasyMall.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Services.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            TenantMap();
            CategoryMap();
            ProductMap();
            ProductPriceMap();
            CartMap();
            OrderMap();
            OrderDetailMap();
            ReviewMap();
        }

        public void TenantMap()
        {
            CreateMap<Tenant, TenantDTO>().ReverseMap();
        }

        public void CategoryMap()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
        }

        public void ProductMap()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
        }

        public void ProductPriceMap()
        {
            CreateMap<ProductPrice, ProductPriceDTO>().ReverseMap();
        }

        public void CartMap() 
        {
            CreateMap<Cart, CartDTO>().ReverseMap();
        }

        public void OrderMap()
        {
            CreateMap<Order, OrderDTO>().ReverseMap();
        }

        public void OrderDetailMap()
        {
            CreateMap<OrderDetail, OrderDetail>().ReverseMap();
        }

        public void ReviewMap()
        {
            CreateMap<Review, ReviewDTO>().ReverseMap();
        }
    }
}
