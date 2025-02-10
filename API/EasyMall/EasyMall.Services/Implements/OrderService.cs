using AutoMapper;
using EasyMall.Commons.Enums;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using EasyMall.Services.Interfaces;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyMall.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IMapper mapper, UserManager<ApplicationUser> userManager,
            ICartRepository cartRepository, IHttpContextAccessor httpContextAccessor,
            IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _cartRepository = cartRepository;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<AppResponse<OrderResponse>> Create(OrderRequest request)
        {
            var result = new AppResponse<OrderResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext?.User.Identity?.Name!);
                var carts = _cartRepository.FindByAsync(c => request.CartIds!.Contains(c.Id)
                                                && c.TenantId == user!.TenantId && c.IsDeleted == false)
                                            .Include(c => c.Product).ThenInclude(c => c!.ProductPrices).ToList();
                var newOrder = _mapper.Map<Order>(request);
                newOrder.Id = Guid.NewGuid();
                newOrder.ProductAddress = request.ProductAddress;
                newOrder.TenantId = user?.TenantId;
                newOrder.Status = request.Status;
                newOrder.CreatedOn = DateTime.UtcNow;
                newOrder.CreatedBy = user?.Email;
                if (newOrder.ProductAddress == Address.HaNoi || newOrder.ProductAddress == Address.HoChiMinh || newOrder.ProductAddress == Address.DaNang
                    || newOrder.ProductAddress == Address.HaiPhong || newOrder.ProductAddress == Address.CanTho)
                    newOrder.ShippingMethod = ShippingMethod.Truck;
                else if (newOrder.ProductAddress == Address.NuocNgoai)
                    newOrder.ShippingMethod = ShippingMethod.Plane;

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
                        ProductName = cart.Product!.Name,
                        Quantity = cart.Quantity,
                        TotalAmount = cart.TotalAmount,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = user?.Email
                    };
                    orderDetails.Add(orderDetail);
                }
                newOrder.OrderDetails = orderDetails;
                _orderRepository.Add(newOrder);

                var response = _mapper.Map<OrderResponse>(request);
                result.BuildResult(response, "Order created successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
    }
}
