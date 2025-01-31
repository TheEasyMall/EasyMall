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
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
    }
}
