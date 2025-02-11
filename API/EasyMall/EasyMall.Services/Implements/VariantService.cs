using AutoMapper;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using EasyMall.Services.Interfaces;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EasyMall.Services.Implements
{
    public class VariantService : IVariantService
    {
        private readonly IVariantRepository _variantRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VariantService(IVariantRepository variantRepository, IMapper mapper,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, 
            IProductRepository productRepository)
        {
            _variantRepository = variantRepository;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
        }

        public async Task<AppResponse<VariantResponse>> Create(VariantRequest request)
        {
            var result = new AppResponse<VariantResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext?.User.Identity?.Name!);
                var newPrice = _mapper.Map<Variant>(request);
                newPrice.Id = Guid.NewGuid();
                newPrice.Type = request.Type;
                newPrice.Price = request.Price;
                newPrice.CreatedBy = user?.Email;
                newPrice.CreatedOn = DateTime.UtcNow;
                _variantRepository.Add(newPrice);

                var response = _mapper.Map<VariantResponse>(request);
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
                var productPrice = _variantRepository.FindByAsync(p => p.Id == id).First();
                if (productPrice == null || productPrice.IsDeleted == true)
                    return result.BuildError("Variant of product not found or deleted");
                productPrice!.IsDeleted = true;
                _variantRepository.Edit(productPrice);
                result.BuildResult("Variant of product deleted successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<VariantResponse> Update(VariantRequest request)
        {
            var result = new AppResponse<VariantResponse>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User.Identity?.Name!;
                var productPrice = _variantRepository.FindByAsync(p => p.Id == request.Id).First();
                if (productPrice == null || productPrice.IsDeleted == true)
                    return result.BuildError("Variant of product not found or deleted");
                productPrice!.Price = request.Price;
                productPrice.Type = request.Type;
                productPrice.Modifiedby = user;
                productPrice.ModifiedOn = DateTime.UtcNow;
                _variantRepository.Edit(productPrice);

                var response = _mapper.Map<VariantResponse>(request);
                if (productPrice.ProductId.HasValue)
                {
                    var product = _productRepository.FindByAsync(c => c.Id == productPrice.ProductId.Value)
                        .FirstOrDefault();
                    response.ProductName = product?.Name!;
                }
                result.BuildResult(response, "Variant of product updated successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
