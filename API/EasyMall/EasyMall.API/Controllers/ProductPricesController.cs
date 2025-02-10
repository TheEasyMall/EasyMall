using EasyMall.Models.DTOs.Request;
using EasyMall.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyMall.API.Controllers
{
    [Route("product-price")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, TenantAdmin")]
    public class ProductPricesController : ControllerBase
    {
        private readonly IProductPriceService _productPriceService;

        public ProductPricesController(IProductPriceService productPriceService)
        {
            _productPriceService = productPriceService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductPriceRequest request)
        {
            var result = await _productPriceService.Create(request);
            return Ok(result);
        }

        [HttpPut]
        public IActionResult Update(ProductPriceRequest request)
        {
            var result = _productPriceService.Update(request);
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var result = _productPriceService.Delete(id);
            return Ok(result);
        }
    }
}
