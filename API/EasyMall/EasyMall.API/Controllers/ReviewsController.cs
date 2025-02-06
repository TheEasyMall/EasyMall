using EasyMall.DTO;
using EasyMall.Services.Interfaces;
using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyMall.API.Controllers
{
    [Route("review")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, TenantAdmin")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReviewDTO request)
        {
            var result = await _reviewService.Create(request);
            return Ok(result);
        }

        [HttpPost("search")]
        public IActionResult Search(SearchRequest request)
        {
            var result = _reviewService.Search(request);
            return Ok(result);
        }
    }
}
