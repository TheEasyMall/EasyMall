using EasyMall.DTOs.DTOs;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface ICartService
    {
        Task<AppResponse<CartDTO>> AddToCart(CartDTO request);
        AppResponse<string> RemoveFromCart(Guid productId);
        AppResponse<string> DeleteFromCart(Guid productId);
        AppResponse<SearchResponse<CartDTO>> Search(SearchRequest request);
    }
}
