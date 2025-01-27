using AutoMapper;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs;
using EasyMall.Services.Interfaces;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Services.Implements
{
    public class CartService : ICartService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartService(IProductRepository productRepository, ICartRepository cartRepository,
            ITenantRepository tenantRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager, IProductPriceRepository productPriceRepository)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _productPriceRepository = productPriceRepository;
        }

        public async Task<AppResponse<CartDTO>> AddToCart(CartDTO request)
        {
            var result = new AppResponse<CartDTO>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    throw new Exception("User not found.");

                var product = _productRepository.FindBy(p => p.Id == request.ProductId).FirstOrDefault();
                if (product == null)
                    throw new Exception("Product not found.");

                var existingCart = _cartRepository
                    .FindBy(c => c.ProductId == request.ProductId && c.Type == request.Type && c.TenantId == user.TenantId)
                    .FirstOrDefault();

                if (existingCart != null)
                {
                    existingCart.Quantity += request.Quantity;
                    existingCart.TotalAmount = CalculateTotalAmount(request.ProductId!.Value, request.ProductPriceId!.Value, request.Type, existingCart.Quantity);
                    _cartRepository.Delete(existingCart);

                    var updatedCart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        ProductId = existingCart.ProductId,
                        Type = existingCart.Type,
                        Quantity = existingCart.Quantity,
                        TotalAmount = existingCart.TotalAmount,
                        TenantId = user.TenantId,
                        ProductPriceId = request.ProductPriceId,
                        OrderId = existingCart.OrderId,
                        CreatedBy = user.Email,
                        Modifiedby = user.Email,
                        ModifiedOn = DateTime.UtcNow
                    };
                    _cartRepository.Add(updatedCart);

                    result.BuildResult(_mapper.Map<CartDTO>(updatedCart));
                }
                else
                {
                    var newCart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        ProductId = request.ProductId,
                        Type = request.Type,
                        Quantity = request.Quantity,
                        TotalAmount = CalculateTotalAmount(request.ProductId!.Value, request.ProductPriceId!.Value, request.Type, request.Quantity),
                        TenantId = user.TenantId,
                        ProductPriceId = request.ProductPriceId,
                        OrderId = request.OrderId,
                        CreatedBy = user?.Email,
                        CreatedOn = DateTime.UtcNow
                    };
                    _cartRepository.Add(newCart);
                    result.BuildResult(_mapper.Map<CartDTO>(newCart));
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
            var product = _productRepository.FindBy(p => p.Id == productId).FirstOrDefault();
            if (product == null)
                throw new Exception("Product not found.");

            var productPrice = _productPriceRepository.FindBy(p => p.Id == productPriceId && p.Type == type).FirstOrDefault();
            if (productPrice == null)
                throw new Exception("Product price not found for the given type.");

            return (product.Price + productPrice.Price) * quantity;
        }

        public AppResponse<string> RemoveFromCart(Guid tenantId, Guid productId)
        {
            throw new NotImplementedException();
        }
    }
}
