using EasyMall.DTOs.DTOs;
using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IProductService
    {
        AppResponse<ProductResponse> GetById(Guid id);
        Task<AppResponse<ProductResponse>> Create(ProductRequest request);
        AppResponse<ProductResponse> Update(ProductRequest request);
        AppResponse<string> Delete(Guid id);
        AppResponse<SearchResponse<ProductResponse>> Search(SearchRequest request);
    }
}
