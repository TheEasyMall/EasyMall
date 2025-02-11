using EasyMall.Models.DTOs.Request;
using EasyMall.Services.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyMall.API.Controllers
{
    [Route("variant")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, TenantAdmin")]
    public class VariantsController : ControllerBase
    {
        private readonly VariantService _variantService;

        public VariantsController(VariantService variantService)
        {
            _variantService = variantService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(VariantRequest request)
        {
            var result = await _variantService.Create(request);
            return Ok(result);
        }

        [HttpPut]
        public IActionResult Update(VariantRequest request)
        {
            var result = _variantService.Update(request);
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var result = _variantService.Delete(id);
            return Ok(result);
        }
    }
}
