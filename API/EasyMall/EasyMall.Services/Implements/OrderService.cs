using AutoMapper;
using EasyMall.Commons.Enums;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs;
using EasyMall.Services.Interfaces;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EasyMall.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IMapper mapper, UserManager<ApplicationUser> userManager,
            ICartRepository cartRepository,IHttpContextAccessor httpContextAccessor, 
            IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
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
                var carts = _cartRepository.FindByAsync(c => request.CartIds!.Contains(c.Id)
                                        && c.TenantId == user!.TenantId && c.IsDeleted == false).ToList();
                if (carts == null || !carts.Any())
                    return result.BuildError("Carts not found");

                var newOrder = _mapper.Map<Order>(request);
                newOrder.Id = Guid.NewGuid();
                newOrder.TenantId = user?.TenantId;
                newOrder.Status = Status.Pending;
                newOrder.CreatedOn = DateTime.UtcNow;
                newOrder.CreatedBy = user?.Email;
                newOrder.ShippingMethod = ShippingMethod.Truck;

                var totalAmount = carts.Sum(c => c.TotalAmount);
                newOrder.TotalAmount = totalAmount;
                newOrder.ShippingFee = request.ShippingFee;
                newOrder.ShippingAddress = request.ShippingAddress;
                if (newOrder.ShippingMethod == ShippingMethod.Truck ||
                    newOrder.ShippingMethod == ShippingMethod.Ship)
                    newOrder.TotalAmount += 10000;
                else if (newOrder.ShippingMethod == ShippingMethod.Plane)
                    newOrder.TotalAmount += 20000;
                else if (newOrder.ShippingMethod == ShippingMethod.Mototbike)
                    newOrder.TotalAmount += 5000;

                var orderDetails = new List<OrderDetail>();
                foreach (var cart in carts)
                {
                    cart.IsDeleted = true;
                    _cartRepository.Edit(cart);

                    var orderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = newOrder.Id,
                        ProductId = cart.ProductId,
                        Quantity = cart.Quantity,
                        TotalAmount = cart.TotalAmount,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = user?.Email
                    };
                    orderDetails.Add(orderDetail);
                }
                newOrder.OrderDetails = orderDetails;

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
