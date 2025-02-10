using EasyMall.Models.DTOs.Request;
using EasyMall.Services.Interfaces;
using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyMall.API.Controllers
{
    [Route("cart")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, TenantAdmin")]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CartRequest request)
        {
            var result = await _cartService.AddToCart(request);
            return Ok(result);
        }

        [HttpDelete("remove")]
        public IActionResult RemoveFromCart(Guid productId)
        {
            var result = _cartService.RemoveFromCart(productId);
            return Ok(result);
        }

        [HttpDelete("delete")]
        public IActionResult DeleteFromCart(Guid productId)
        {
            var result = _cartService.DeleteFromCart(productId);
            return Ok(result);
        }
    }
}
