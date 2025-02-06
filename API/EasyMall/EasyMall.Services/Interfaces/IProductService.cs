using EasyMall.DTO;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IProductService
    {
        AppResponse<ProductDTO> GetById(Guid id);
        Task<AppResponse<ProductDTO>> Create(ProductDTO request);
        AppResponse<ProductDTO> Update(ProductDTO request);
        AppResponse<string> Delete(Guid id);
        AppResponse<SearchResponse<ProductDTO>> Search(SearchRequest request);
    }
}
