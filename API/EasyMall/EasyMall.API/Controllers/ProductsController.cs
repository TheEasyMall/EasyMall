using EasyMall.DTO;
using EasyMall.Services.Interfaces;
using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyMall.API.Controllers
{
    [Route("product")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, TenantAdmin")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetById(Guid id)
        {
            var result = _productService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO request)
        {
            var result = await _productService.Create(request);
            return Ok(result);
        }

        [HttpPut]
        public IActionResult Update(ProductDTO request)
        {
            var result = _productService.Update(request);
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var result = _productService.Delete(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search(SearchRequest request)
        {
            var result = _productService.Search(request);
            return Ok(result);
        }
    }
}
