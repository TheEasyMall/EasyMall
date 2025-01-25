using EasyMall.Models.DTOs;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
