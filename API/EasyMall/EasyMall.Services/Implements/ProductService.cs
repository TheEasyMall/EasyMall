using EasyMall.Models.DTOs;
using EasyMall.Services.Interfaces;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace EasyMall.Services.Implements
{
    public class ProductService : IProductService
    {
        public AppResponse<ProductDTO> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<AppResponse<ProductDTO>> Create(ProductDTO request)
        {
            throw new NotImplementedException();
        }

        public AppResponse<ProductDTO> Update(ProductDTO request)
        {
            throw new NotImplementedException();
        }

        public AppResponse<string> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public AppResponse<SearchResponse<ProductDTO>> Search(SearchRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
