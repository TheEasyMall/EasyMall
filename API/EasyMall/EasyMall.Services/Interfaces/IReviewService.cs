using EasyMall.DTOs.DTOs;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IReviewService
    {
        Task<AppResponse<ReviewDTO>> Create(ReviewDTO request);
        AppResponse<SearchResponse<ReviewDTO>> Search(SearchRequest request);
    }
}
