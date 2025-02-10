using EasyMall.Models.DTOs.Request;
using EasyMall.Services.Interfaces;
using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [Route("present")]
        public IActionResult GetByPresent()
        {
            var result = _categoryService.GetByPresent();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById(Guid id)
        {
            var result = _categoryService.GetById(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("list-product/{categoryId}")]
        public IActionResult GetListProductByCategoryId(Guid categoryId)
        {
            var result = _categoryService.GetListProductByCategoryId(categoryId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            var result = await _categoryService.Create(request);
            return Ok(result);
        }

        [HttpPut]
        public IActionResult Update(CategoryRequest request)
        {
            var result = _categoryService.Update(request);
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var result = _categoryService.Delete(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search(SearchRequest request)
        {
            var result = _categoryService.Search(request);
            return Ok(result);
        }
    }
}
