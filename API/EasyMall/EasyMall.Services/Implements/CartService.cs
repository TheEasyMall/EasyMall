using AutoMapper;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using EasyMall.Services.Interfaces;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace EasyMall.Services.Implements
{
    public class CartService : ICartService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartService(IProductRepository productRepository, ICartRepository cartRepository,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, 
            IProductPriceRepository productPriceRepository)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _productPriceRepository = productPriceRepository;
        }

        public async Task<AppResponse<CartResponse>> AddToCart(CartRequest request)
        {
            var result = new AppResponse<CartResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext?.User.Identity?.Name!);
                var product = _productRepository.FindByAsync(p => p.Id == request.ProductId).FirstOrDefault();
                if (product == null || product.IsDeleted == true)
                    return result.BuildError("Product not found or deleted");

                var existingCart = _cartRepository
                    .FindByAsync(c => c.ProductId == request.ProductId && c.Type == request.Type
                    && c.TenantId == user!.TenantId && c.IsDeleted == false).FirstOrDefault();
                if (existingCart != null)
                {
                    existingCart.Quantity += request.Quantity;
                    existingCart.TotalAmount = CalculateTotalAmount(request.ProductId!.Value, request.ProductPriceId!.Value,
                        request.Type, existingCart.Quantity);
                    _cartRepository.Delete(existingCart);
                    var updatedCart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        ProductId = existingCart.ProductId,
                        Type = existingCart.Type,
                        Quantity = existingCart.Quantity,
                        TotalAmount = existingCart.TotalAmount,
                        TenantId = user!.TenantId,
                        ProductPriceId = request.ProductPriceId,
                        CreatedBy = user.Email,
                        Modifiedby = user.Email,
                        ModifiedOn = DateTime.UtcNow
                    };
                    _cartRepository.Add(updatedCart);

                    var response = _mapper.Map<CartResponse>(updatedCart);
                    response.ProductName = product.Name;
                    var productPrice = _productPriceRepository.FindByAsync(p => p.Id == request.ProductPriceId).FirstOrDefault();
                    response.ProductPriceName = productPrice?.Type!;
                    result.BuildResult(response, "Cart updated successfully");
                }
                else
                {
                    var newCart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        ProductId = request.ProductId,
                        Type = request.Type,
                        Quantity = request.Quantity,
                        TotalAmount = CalculateTotalAmount(request.ProductId!.Value, request.ProductPriceId!.Value,
                            request.Type, request.Quantity),
                        TenantId = user!.TenantId,
                        ProductPriceId = request.ProductPriceId,
                        CreatedBy = user?.Email,
                        CreatedOn = DateTime.UtcNow
                    };
                    _cartRepository.Add(newCart);

                    var response = _mapper.Map<CartResponse>(newCart);
                    response.ProductName = product.Name;
                    var productPrice = _productPriceRepository.FindByAsync(p => p.Id == request.ProductPriceId).FirstOrDefault();
                    response.ProductPriceName = productPrice?.Type!;
                    result.BuildResult(response, "Added product to cart successfully");
                }
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        private double CalculateTotalAmount(Guid productId, Guid productPriceId, string type, int quantity)
        {
            var product = _productRepository.FindByAsync(p => p.Id == productId).FirstOrDefault();
            if (product == null || product.IsDeleted == true)
                throw new Exception("Product not found or deleted");

            var productPrice = _productPriceRepository.FindByAsync(p => p.Id == productPriceId && p.Type == type).FirstOrDefault();
            if (productPrice == null || productPrice.IsDeleted == true)
                throw new Exception("Product price not found or deleted for the given type");
            return (product.Price + productPrice.Price) * quantity;
        }

        public AppResponse<string> RemoveFromCart(Guid productId)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User.Identity?.Name!;
                var cart = _cartRepository.FindByAsync(c => c.ProductId == productId && c.IsDeleted == false).FirstOrDefault();
                if (cart == null || cart.IsDeleted == true)
                    return result.BuildError("Product not found or deleted in the cart");
                if (cart.Quantity > 1)
                {
                    cart.Quantity -= 1;
                    cart.TotalAmount = CalculateTotalAmount(cart.ProductId!.Value, cart.ProductPriceId!.Value, cart.Type, cart.Quantity);
                    _cartRepository.Edit(cart);
                    result.BuildResult("Product quantity decreased successfully");
                }
                else
                {
                    cart.Quantity = 0;
                    cart.IsDeleted = true;
                    _cartRepository.Edit(cart);
                    result.BuildResult("Product removed from cart successfully");
                }
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<string> DeleteFromCart(Guid productId)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User.Identity?.Name!;
                var cart = _cartRepository.FindByAsync(c => c.ProductId == productId && c.IsDeleted == false).FirstOrDefault();
                if (cart == null || cart.IsDeleted == true)
                    return result.BuildError("Product not found or deleted in the cart");
                cart.IsDeleted = true;
                _cartRepository.Edit(cart);
                result.BuildResult("Product removed from cart successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }
    }
}
