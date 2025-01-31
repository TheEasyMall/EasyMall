using AutoMapper;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Implements;
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
    public class ProductPriceService : IProductPriceService
    {
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductPriceService(IProductPriceRepository productPriceRepository, IMapper mapper, 
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _productPriceRepository = productPriceRepository;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppResponse<ProductPriceDTO>> Create(ProductPriceDTO request)
        {
            var result = new AppResponse<ProductPriceDTO>();
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
                result.BuildResult(request);
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
                if (productPrice == null)
                    result.BuildError("Product not found");
                productPrice!.IsDeleted = true;
                _productPriceRepository.Edit(productPrice);
                result.BuildResult("Delete product price successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<ProductPriceDTO> Update(ProductPriceDTO request)
        {
            var result = new AppResponse<ProductPriceDTO>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User.Identity?.Name!;
                var productPrice = _productPriceRepository.FindByAsync(p => p.Id == request.Id).First();
                if (productPrice == null)
                    result.BuildError("Product price not found");
                productPrice!.Price = request.Price;
                productPrice.Type = request.Type;
                productPrice.Modifiedby = user;
                productPrice.ModifiedOn = DateTime.UtcNow;
                _productPriceRepository.Edit(productPrice);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
