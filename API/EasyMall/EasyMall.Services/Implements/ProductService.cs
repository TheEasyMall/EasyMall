using AutoMapper;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Implements;
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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;
        }

        public AppResponse<ProductDTO> GetById(Guid id)
        {
            var result = new AppResponse<ProductDTO>();
            try
            {
                var product = _productRepository.FindByAsync(p => p.Id == id).First();
                if (product == null)
                    result.BuildError("Product not found");
                var dto = _mapper.Map<ProductDTO>(product);
                result.BuildResult(dto);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<ProductDTO>> Create(ProductDTO request)
        {
            var result = new AppResponse<ProductDTO>();
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
                result.BuildResult(request, "Product created successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result; 
        }

        public AppResponse<ProductDTO> Update(ProductDTO request)
        {
            var result = new AppResponse<ProductDTO>();
            try
            {
                var user = _httpContextAccessor.HttpContext?.User.Identity?.Name!;
                var product = _productRepository.FindByAsync(p => p.Id == request.Id).First();
                if (product == null)
                    result.BuildError("Product not found");

                product!.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.Quantity = request.Quantity;
                product.Address = request.Address;
                product.CategoryId = request.CategoryId;
                product.Modifiedby = user;
                product.ModifiedOn = DateTime.UtcNow;

                _productRepository.Edit(product);
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

        public AppResponse<SearchResponse<ProductDTO>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ProductDTO>>();
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
                var dtoList = _mapper.Map<List<ProductDTO>>(categoryList);
                var searchResponse = new SearchResponse<ProductDTO>
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
                            case "Price":
                                predicate = predicate.And(x => x.Price == double.Parse(filter.Value));
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
