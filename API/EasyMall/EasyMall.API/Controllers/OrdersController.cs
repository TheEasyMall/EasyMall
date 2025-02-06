using EasyMall.DTOs.DTOs;
using EasyMall.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyMall.API.Controllers
{
    [Route("order")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, TenantAdmin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderDTO request)
        {
            var result = await _orderService.Create(request);
            return Ok(result);
        }
    }
}
