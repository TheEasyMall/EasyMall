using EasyMall.DTOs.DTOs;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IProductPriceService
    {
        Task<AppResponse<ProductPriceDTO>> Create(ProductPriceDTO request);
        AppResponse<ProductPriceDTO> Update(ProductPriceDTO request);
        AppResponse<string> Delete(Guid id);
    }
}
