using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IProductPriceService
    {
        Task<AppResponse<ProductPriceResponse>> Create(ProductPriceRequest request);
        AppResponse<ProductPriceResponse> Update(ProductPriceRequest request);
        AppResponse<string> Delete(Guid id);
    }
}
