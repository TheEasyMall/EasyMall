using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface ICartService
    {
        Task<AppResponse<CartResponse>> AddToCart(CartRequest request);
        AppResponse<string> RemoveFromCart(Guid productId);
        AppResponse<string> DeleteFromCart(Guid productId);
    }
}
