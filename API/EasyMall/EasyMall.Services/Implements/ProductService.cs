using AutoMapper;
using Azure;
using EasyMall.Commons.Enums;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using EasyMall.Services.Interfaces;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace EasyMall.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public AppResponse<ProductResponse> GetById(Guid id)
        {
            var result = new AppResponse<ProductResponse>();
            try
            {
                var product = _productRepository.FindByAsync(p => p.Id == id).FirstOrDefault();
                if (product == null || product.IsDeleted == true)
                    return result.BuildError("Product not found or deleted");
                var dto = _mapper.Map<ProductResponse>(product);
                if (product.CategoryId.HasValue)
                {
                    var category = _categoryRepository.FindByAsync(c => c.Id == product.CategoryId.Value)
                        .FirstOrDefault();
                    dto.CategoryName = category?.Name!;
                }
                result.BuildResult(dto);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<ProductResponse>> Create(ProductRequest request)
        {
            var result = new AppResponse<ProductResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext?.User.Identity?.Name!);
                var newProduct = _mapper.Map<Product>(request);
                newProduct.Id = Guid.NewGuid();
                newProduct.Name = request.Name;
                newProduct.Description = request.Description;
                newProduct.Price = request.Price;
                newProduct.Quantity = request.Quantity;
                newProduct.Address = request.Address;
                newProduct.TenantId = user?.TenantId;
                newProduct.CategoryId = request.CategoryId;
                newProduct.CreatedBy = user?.Email;
                newProduct.CreatedOn = DateTime.UtcNow;
                _productRepository.Add(newProduct);

                var response = _mapper.Map<ProductResponse>(newProduct);
                if (newProduct.CategoryId.HasValue)
                {
                    var category = _categoryRepository.FindByAsync(c => c.Id == newProduct.CategoryId.Value)
                        .FirstOrDefault();
                    response.CategoryName = category?.Name!;
                }
                result.BuildResult(response, "Product created successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<ProductResponse> Update(ProductRequest request)
        {
            var result = new AppResponse<ProductResponse>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User.Identity?.Name!;
                var product = _productRepository.Get(request.Id!.Value);
                if (product == null || product.IsDeleted == true)
                    return result.BuildError("Product not found of deleted");

                product!.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.Quantity = request.Quantity;
                product.Address = request.Address;
                product.CategoryId = request.CategoryId;
                product.Modifiedby = user;
                product.ModifiedOn = DateTime.UtcNow;
                _productRepository.Edit(product);

                var response = _mapper.Map<ProductResponse>(request);
                if (product.CategoryId.HasValue)
                {
                    var category = _categoryRepository.FindByAsync(c => c.Id == product.CategoryId.Value)
                        .FirstOrDefault();
                    response.CategoryName = category?.Name!;
                }
                result.BuildResult(response);
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
                var product = _productRepository.FindByAsync(p => p.Id == id).First();
                if (product == null)
                    result.BuildError("Product not found");
                product!.IsDeleted = true;
                _productRepository.Edit(product);
                result.BuildResult("Delete product successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<ProductResponse>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ProductResponse>>();
            try
            {
                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = _productRepository.CountRecordsByPredicate(query);
                var products = _productRepository.FindByPredicate(query);
                if (request.SortBy != null)
                    products = _productRepository.addSort(products, request.SortBy);
                else
                    products = products.OrderBy(x => x.Name);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * pageSize;
                var categoryList = products.Skip(startIndex).Take(pageSize);
                var dtoList = _mapper.Map<List<ProductResponse>>(categoryList);
                var searchResponse = new SearchResponse<ProductResponse>
                {
                    TotalRows = numOfRecords,
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    CurrentPage = pageIndex,
                    Data = dtoList,
                };
                foreach (var product in dtoList)
                {
                    if (product.CategoryId.HasValue)
                    {
                        var category = _categoryRepository.FindByAsync(c => c.Id == product.CategoryId.Value)
                            .FirstOrDefault();
                        product.CategoryName = category?.Name!;
                    }
                }
                result.BuildResult(searchResponse);
                result.Data = searchResponse;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private ExpressionStarter<Product> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Product>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "Name":
                                predicate = predicate.And(x => x.Name.Contains(filter.Value));
                                break;
                            case "Address":
                            {
                                 var enumN = int.Parse(filter.Value);
                                 var emunType = (Address)enumN;
                                 predicate = predicate.And(m => m.Address.Equals(emunType));
                                 break;
                            }
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
