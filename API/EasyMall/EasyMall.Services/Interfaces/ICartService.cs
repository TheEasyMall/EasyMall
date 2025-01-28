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
    public interface ICartService
    {
        Task<AppResponse<CartDTO>> AddToCart(CartDTO request);
        AppResponse<string> RemoveFromCart(Guid productId);
        AppResponse<SearchResponse<CartDTO>> Search(SearchRequest request);
    }
}
