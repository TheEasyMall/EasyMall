using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IReviewService
    {
        Task<AppResponse<ReviewResponse>> Create(ReviewRequest request);
        AppResponse<SearchResponse<ReviewResponse>> Search(SearchRequest request);
    }
}
