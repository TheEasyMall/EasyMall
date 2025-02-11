using AutoMapper;
using EasyMall.DALs.Entities;
using EasyMall.DTOs.DTOs;
using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;

namespace EasyMall.Services.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap();
        }

        public void CreateMap()
        {
            CreateMap<Tenant, TenantDTO>().ReverseMap();

            #region Entity - Request
            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Product, ProductRequest>().ReverseMap();
            CreateMap<Variant, VariantRequest>().ReverseMap();
            CreateMap<Cart, CartRequest>().ReverseMap();
            CreateMap<Order, OrderRequest>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailRequest>().ReverseMap();
            CreateMap<Review, ReviewRequest>().ReverseMap();
            #endregion

            #region Entity - Response
            CreateMap<Category,CategoryResponse>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();
            CreateMap<Variant, VariantResponse>().ReverseMap();
            CreateMap<Cart, CartResponse>().ReverseMap();
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailResponse>().ReverseMap();
            CreateMap<Review, ReviewResponse>().ReverseMap();
            #endregion

            #region Request - Response
            CreateMap<CategoryRequest, CategoryResponse>().ReverseMap();
            CreateMap<ProductRequest, ProductResponse>().ReverseMap();
            CreateMap<VariantRequest, VariantResponse>().ReverseMap();
            CreateMap<CartRequest, CartResponse>().ReverseMap();
            CreateMap<OrderRequest, OrderResponse>().ReverseMap();
            CreateMap<OrderDetailRequest, OrderDetailResponse>().ReverseMap();
            CreateMap<ReviewRequest, ReviewResponse>().ReverseMap();
            #endregion
        }
    }
}
