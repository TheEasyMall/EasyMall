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
using Microsoft.EntityFrameworkCore;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace EasyMall.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoryService(ICategoryRepository categoryRepository, IHttpContextAccessor contextAccessor,
            IMapper mapper, IProductRepository productRepository,
            UserManager<ApplicationUser> userManager)
        {
            _categoryRepository = categoryRepository;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _productRepository = productRepository;
            _userManager = userManager;
        }

        public AppResponse<List<CategoryDTO>> GetByPresent()
        {
            var result = new AppResponse<List<CategoryDTO>>();
            try
            {
                var categories = _categoryRepository.FindByAsync(x => x.IsPresent == true).Include(x => x.Products).ToList();
                var dtos = _mapper.Map<List<CategoryDTO>>(categories);
                if (categories == null || !categories.Any())
                    result.BuildError("Category not found");
                else
                    result.BuildResult(dtos);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<CategoryDTO> GetById(Guid id)
        {
            var result = new AppResponse<CategoryDTO>();
            try
            {
                var category = _categoryRepository.FindByAsync(x => x.Id == id).Include(x => x.Products).First();
                var dto = _mapper.Map<CategoryDTO>(category);
                if (category == null || category.IsDeleted == true)
                    result.BuildError("Category not found");
                result.BuildResult(dto);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<CategoryDTO>> Create(CategoryDTO request)
        {
            var result = new AppResponse<CategoryDTO>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                var newCategory = _mapper.Map<Category>(request);
                newCategory.Id = Guid.NewGuid();
                newCategory.Name = request.Name;
                newCategory.Description = request.Description;
                newCategory.IsPresent = true;
                newCategory.CreatedOn = DateTime.UtcNow;
                newCategory.CreatedBy = user?.Email;
                newCategory.TenantId = user?.TenantId;

                var newListProduct = new List<Product>();
                foreach (var product in newListProduct)
                {
                    product.Id = Guid.NewGuid();
                    product.Name = product.Name;
                    product.Description = product.Description;
                    product.Price = product.Price;
                    product.Quantity = product.Quantity;
                    product.CreatedOn = DateTime.UtcNow;
                    product.CreatedBy = user?.Email;
                    product.CategoryId = newCategory.Id;
                    newListProduct.Add(product);
                }

                _categoryRepository.AddProductToCategory(newCategory, newListProduct);
                result.BuildResult(request, "Product created successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<CategoryDTO> Update(CategoryDTO request)
        {
            var result = new AppResponse<CategoryDTO>();
            try
            {
                var user = _contextAccessor.HttpContext?.User.Identity?.Name!;
                var category = _categoryRepository.FindByAsync(x => x.Id == request.Id).Include(x => x.Products).First();
                if (category == null)
                    return result.BuildError("Category not found");

                var product = _productRepository.FindByAsync(x => x.CategoryId == category.Id).ToList();
                product.ForEach(x =>
                {
                    var dto = request.Products?.FirstOrDefault(dto => dto.Id == x.Id);
                    if (dto == null)
                        x.IsDeleted = true;
                    else
                    {
                        var change = AreProductsEqual(dto, x);
                        if (!change)
                        {
                            x.IsDeleted = false;
                            x.Name = dto.Name;
                            x.Description = dto.Description;
                            x.Price = dto.Price;
                            x.Quantity = dto.Quantity;
                            x.CreatedBy = user;
                            x.CreatedOn = DateTime.UtcNow;
                        }
                    }
                });

                var newListProduct = new List<Product>();
                var newProducts = request.Products?.Where(x => x.Id == null).ToList();
                newProducts?.ForEach(x =>
                {
                    var newProduct = _mapper.Map<Product>(x);
                    newProduct.Id = Guid.NewGuid();
                    newProduct.Name = x.Name;
                    newProduct.Description = x.Description;
                    newProduct.Price = x.Price;
                    newProduct.Quantity = x.Quantity;
                    newProduct.CategoryId = category.Id;
                    newProduct.CreatedBy = user;
                    newProduct.CreatedOn = DateTime.UtcNow;
                    newListProduct.Add(newProduct);
                });

                category.Name = request.Name;
                category.Description = request.Description;
                category.Modifiedby = user;
                category.ModifiedOn = DateTime.UtcNow;

                _categoryRepository.UpdateProductOnCategory(category, product, newListProduct);
                result.BuildResult(request);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private bool AreProductsEqual(ProductDTO productDTO, Product product)
        {
            if (productDTO == null || product == null)
                return false;
            return productDTO.Name == product.Name &&
                productDTO.Description == product.Description &&
                productDTO.Price == product.Price &&
                productDTO.Quantity == product.Quantity;
        }

        public AppResponse<string> Delete(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var category = _categoryRepository.Get(id);
                if (category == null)
                    result.BuildError("Category not found");
                category!.IsDeleted = true;
                _categoryRepository.Edit(category);
                result.BuildResult("Delete category successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<CategoryDTO>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<CategoryDTO>>();
            try
            {
                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = _categoryRepository.CountRecordsByPredicate(query);
                var categories = _categoryRepository.FindByPredicate(query).Include(x => x.Products).AsQueryable();
                if (request.SortBy != null)
                    categories = _categoryRepository.addSort(categories, request.SortBy);
                else
                    categories = categories.OrderBy(x => x.Name);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * pageSize;
                var categoryList = categories.Skip(startIndex).Take(pageSize).ToList();
                var dtoList = _mapper.Map<List<CategoryDTO>>(categoryList);
                var searchResponse = new SearchResponse<CategoryDTO>
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

        private ExpressionStarter<Category> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Category>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "Name":
                                predicate = predicate.And(x => x.Name.Contains(filter.Value));
                                break;
                            case "IsPresent":
                                predicate = predicate.And(x => x.IsPresent == bool.Parse(filter.Value));
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
