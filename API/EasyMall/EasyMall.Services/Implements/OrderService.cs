using AutoMapper;
using EasyMall.Commons.Enums;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs;
using EasyMall.Services.Interfaces;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IMapper mapper, UserManager<ApplicationUser> userManager,
            ITenantRepository tenantRepository, ICartRepository cartRepository,
            IHttpContextAccessor httpContextAccessor, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _tenantRepository = tenantRepository;
            _cartRepository = cartRepository;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
        }

        public async Task<AppResponse<OrderDTO>> Create(OrderDTO request)
        {
            var result = new AppResponse<OrderDTO>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext?.User.Identity?.Name!);
                var carts = _cartRepository.FindByAsync(c => request.CartIds!.Contains(c.Id) && c.TenantId == user!.TenantId).ToList();
                if (carts == null || !carts.Any())
                    return result.BuildError("Carts not found");

                var totalAmount = carts.Sum(c => c.TotalAmount);
                var newOrder = _mapper.Map<Order>(request);
                newOrder.Id = Guid.NewGuid();
                newOrder.TenantId = user?.TenantId;
                newOrder.Status = Status.Pending;
                newOrder.CreatedOn = DateTime.UtcNow;
                newOrder.CreatedBy = user?.Email;
                newOrder.TotalAmount = totalAmount;
                newOrder.ShippingFee = request.ShippingFee;
                newOrder.ShippingMethod = request.ShippingMethod;
                newOrder.ShippingAddress = request.ShippingAddress;
                _orderRepository.Add(newOrder);

                result.BuildResult(request, "Order created successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
    }
}
