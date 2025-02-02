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
    public interface IReviewService
    {
        Task<AppResponse<ReviewDTO>> Create(ReviewDTO request);
        AppResponse<SearchResponse<ReviewDTO>> Search(SearchRequest request);
    }
}
