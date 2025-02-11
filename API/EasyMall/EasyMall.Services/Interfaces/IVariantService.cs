using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IVariantService
    {
        Task<AppResponse<VariantResponse>> Create(VariantRequest request);
        AppResponse<VariantResponse> Update(VariantRequest request);
        AppResponse<string> Delete(Guid id);
    }
}
