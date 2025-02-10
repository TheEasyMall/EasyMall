using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IOrderService
    {
        Task<AppResponse<OrderResponse>> Create(OrderRequest request);
    }
}
