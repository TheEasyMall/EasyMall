using AutoMapper;
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

        public AppResponse<List<CategoryResponse>> GetByPresent()
        {
            var result = new AppResponse<List<CategoryResponse>>();
            try
            {
                var categories = _categoryRepository.FindByAsync(x => x.IsPresent == true && x.IsDeleted == false)
                    .Include(x => x.Products).ToList();
                var dtos = _mapper.Map<List<CategoryResponse>>(categories);
                if (categories == null || !categories.Any())
                    result.BuildError("Category not found or deleted");
                else
                    result.BuildResult(dtos);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<CategoryResponse> GetById(Guid id)
        {
            var result = new AppResponse<CategoryResponse>();
            try
            {
                var category = _categoryRepository.Get(id);
                if (category == null || category.IsDeleted == true)
                    return result.BuildError("Category not found or deleted");

                var data = new CategoryResponse
                {
                    Id = id,
                    Name = category!.Name,
                    Description = category.Description,
                    IsPresent = category.IsPresent,
                    Products = _productRepository.FindByAsync(x => x.CategoryId == category.Id)
                        .Select(x => new ProductResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            Price = x.Price,
                            Quantity = x.Quantity,
                            Address = x.Address,
                            CategoryId = category.Id,
                            CategoryName = category.Name,
                        }).ToList()
                };
                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<List<ProductResponse>> GetListProductByCategoryId(Guid categoryId)
        {
            var result = new AppResponse<List<ProductResponse>>();
            try
            {
                var data = _productRepository.FindByAsync(x => x.CategoryId == categoryId && x.IsDeleted == false)
                    .Include(x => x.Category).Select(x => new ProductResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Address = x.Address,
                        CategoryId = categoryId,
                        CategoryName = x.Category!.Name,
                    }).ToList();
                result.BuildResult(data);
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<CategoryResponse>> Create(CategoryRequest request)
        {
            var result = new AppResponse<CategoryResponse>();
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
                _categoryRepository.Add(newCategory);

                var response = _mapper.Map<CategoryResponse>(newCategory);
                result.BuildResult(response, "Product created successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<CategoryResponse> Update(CategoryRequest request)
        {
            var result = new AppResponse<CategoryResponse>();
            try
            {
                var user = _contextAccessor.HttpContext?.User.Identity?.Name!;
                var category = _categoryRepository.Get(request.Id!.Value);
                if (category == null || category.IsDeleted == true)
                    return result.BuildError("Category not found or deleted");

                category.Name = request.Name;
                category.Description = request.Description;
                category.IsPresent = request.IsPresent;
                category.Modifiedby = user;
                category.ModifiedOn = DateTime.UtcNow;
                _categoryRepository.Edit(category);

                var response = _mapper.Map<CategoryResponse>(category);
                response.Products = _productRepository.FindByAsync(x => x.CategoryId == category.Id)
                    .Select(x => new ProductResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Address = x.Address,
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                    }).ToList();
                result.BuildResult(response, "Category updated successfully");
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
                var category = _categoryRepository.Get(id);
                if (category == null)
                    result.BuildError("Category not found");
                category!.IsDeleted = true;
                _categoryRepository.Edit(category);
                result.BuildResult("Category deleted successfully");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<CategoryResponse>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<CategoryResponse>>();
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
                var dtoList = _mapper.Map<List<CategoryResponse>>(categoryList);
                var searchResponse = new SearchResponse<CategoryResponse>
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
