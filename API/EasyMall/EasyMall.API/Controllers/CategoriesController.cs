using EasyMall.Models.DTOs;
using EasyMall.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyMall.API.Controllers
{
    [Route("category")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, TenantAdmin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO request)
        {
            var result = await _categoryService.Create(request);
            return Ok(result);
        }

        [HttpPut]
        public IActionResult Update(CategoryDTO request)
        {
            var result = _categoryService.Update(request);
            return Ok(result);
        }
    }
}
