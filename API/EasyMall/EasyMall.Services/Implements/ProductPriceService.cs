using AutoMapper;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Implements;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using EasyMall.Services.Interfaces;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EasyMall.Services.Implements
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductPriceService(IProductPriceRepository productPriceRepository, IMapper mapper,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, 
            IProductRepository productRepository)
        {
            _productPriceRepository = productPriceRepository;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
        }

        public async Task<AppResponse<ProductPriceResponse>> Create(ProductPriceRequest request)
        {
            var result = new AppResponse<ProductPriceResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext?.User.Identity?.Name!);
                var newPrice = _mapper.Map<ProductPrice>(request);
                newPrice.Id = Guid.NewGuid();
                newPrice.Type = request.Type;
                newPrice.Price = request.Price;
                newPrice.CreatedBy = user?.Email;
                newPrice.CreatedOn = DateTime.UtcNow;
                _productPriceRepository.Add(newPrice);

                var response = _mapper.Map<ProductPriceResponse>(request);
                if (newPrice.ProductId.HasValue)
                {
                    var product = _productRepository.FindByAsync(c => c.Id == newPrice.ProductId.Value)
                        .FirstOrDefault();
                    response.ProductName = product?.Name!;
                }
                result.BuildResult(response, "The prices of the product categories have been created successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<string> Delete(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var productPrice = _productPriceRepository.FindByAsync(p => p.Id == id).First();
                if (productPrice == null || productPrice.IsDeleted == true)
                    return result.BuildError("Product price not found or deleted");
                productPrice!.IsDeleted = true;
                _productPriceRepository.Edit(productPrice);
                result.BuildResult("Product price deleted successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<ProductPriceResponse> Update(ProductPriceRequest request)
        {
            var result = new AppResponse<ProductPriceResponse>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User.Identity?.Name!;
                var productPrice = _productPriceRepository.FindByAsync(p => p.Id == request.Id).First();
                if (productPrice == null || productPrice.IsDeleted == true)
                    return result.BuildError("Product price not found or deleted");
                productPrice!.Price = request.Price;
                productPrice.Type = request.Type;
                productPrice.Modifiedby = user;
                productPrice.ModifiedOn = DateTime.UtcNow;
                _productPriceRepository.Edit(productPrice);

                var response = _mapper.Map<ProductPriceResponse>(request);
                if (productPrice.ProductId.HasValue)
                {
                    var product = _productRepository.FindByAsync(c => c.Id == productPrice.ProductId.Value)
                        .FirstOrDefault();
                    response.ProductName = product?.Name!;
                }
                result.BuildResult(response, "Product price updated successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
