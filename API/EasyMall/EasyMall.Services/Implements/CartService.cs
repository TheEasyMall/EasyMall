using AutoMapper;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs;
using EasyMall.Services.Interfaces;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
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

        public async Task<AppResponse<CartDTO>> AddToCart(CartDTO request)
        {
            var result = new AppResponse<CartDTO>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext?.User.Identity?.Name!);
                if (user == null)
                    throw new Exception("User not found.");

                var product = _productRepository.FindByAsync(p => p.Id == request.ProductId).FirstOrDefault();
                if (product == null)
                    throw new Exception("Product not found.");

                var existingCart = _cartRepository
                    .FindByAsync(c => c.ProductId == request.ProductId && c.Type == request.Type && c.TenantId == user.TenantId && c.IsDeleted == false)
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
                        CreatedBy = user?.Email,
                        CreatedOn = DateTime.UtcNow
                    };
                    _cartRepository.Add(newCart);
                    result.BuildResult(_mapper.Map<CartDTO>(newCart), "Added product to cart successfully");
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
            if (product == null)
                throw new Exception("Product not found.");

            var productPrice = _productPriceRepository.FindByAsync(p => p.Id == productPriceId && p.Type == type).FirstOrDefault();
            if (productPrice == null)
                throw new Exception("Product price not found for the given type.");

            return (product.Price + productPrice.Price) * quantity;
        }

        public AppResponse<string> RemoveFromCart(Guid productId)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User.Identity?.Name!;
                var cart = _cartRepository.FindByAsync(c => c.ProductId == productId && c.IsDeleted == false).FirstOrDefault();
                if (cart == null)
                    throw new Exception("Product not found in the cart.");
                if (cart.Quantity > 1)
                {
                    cart.Quantity -= 1;
                    cart.TotalAmount = CalculateTotalAmount(cart.ProductId!.Value, cart.ProductPriceId!.Value, cart.Type, cart.Quantity);
                    _cartRepository.Edit(cart);
                    result.BuildResult("Product quantity decreased successfully.");
                }
                else
                {
                    cart.Quantity = 0;
                    cart.IsDeleted = true;
                    _cartRepository.Edit(cart);
                    result.BuildResult("Product removed from cart successfully.");
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
                if (cart == null)
                    throw new Exception("Product not found in the cart.");
                cart.IsDeleted = true;
                _cartRepository.Edit(cart);
                result.BuildResult("Product removed from cart successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message);
            }
            return result;
        }

        public AppResponse<SearchResponse<CartDTO>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<CartDTO>>();
            try
            {
                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = _cartRepository.CountRecordsByPredicate(query);
                var carts = _cartRepository.FindByPredicate(query);
                if (request.SortBy != null)
                    carts = _cartRepository.addSort(carts, request.SortBy);
                else
                    carts = carts.OrderBy(x => x.Product!.Name);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * pageSize;
                var categoryList = carts.Skip(startIndex).Take(pageSize);
                var dtoList = _mapper.Map<List<CartDTO>>(categoryList);
                var searchResponse = new SearchResponse<CartDTO>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = dtoList,
                };
                result.Data = searchResponse;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private ExpressionStarter<Cart> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Cart>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "Name":
                                predicate = predicate.And(x => x.Product!.Name.Contains(filter.Value));
                                break;
                            case "Price":
                                predicate = predicate.And(x => x.Product!.Price == double.Parse(filter.Value));
                                break;
                            default:
                                break;
                        }
                    }
                }

                predicate = predicate.And(x => x.IsDeleted == false);
                return predicate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
