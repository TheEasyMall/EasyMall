﻿using AutoMapper;
using EasyMall.Commons.Enums;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.DTOs.DTOs;
using EasyMall.Models.DTOs.Request;
using EasyMall.Models.DTOs.Response;
using EasyMall.Services.Interfaces;
using LinqKit;
using MayNghien.Infrastructure.Request.Base;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using static Maynghien.Infrastructure.Helpers.SearchHelper;

namespace EasyMall.Services.Implements
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductRepository _productRepository;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper,
            IHttpContextAccessor contextAccessor, UserManager<ApplicationUser> userManager, 
            IProductRepository productRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _productRepository = productRepository;
        }

        public async Task<AppResponse<ReviewResponse>> Create(ReviewRequest request)
        {
            var result = new AppResponse<ReviewResponse>();
            try
            {
                var user = await _userManager.FindByEmailAsync(_contextAccessor.HttpContext?.User.Identity?.Name!);
                var products = _productRepository.FindByAsync(p => p.Id == request.ProductId).ToList();
                if (products == null)
                    result.BuildError("Product not found");

                var newReview = new Review
                {
                    Id = Guid.NewGuid(),
                    Rating = request.Rating,
                    Comment = request.Comment,
                    ProductId = request.ProductId,
                    ProductName = products!.ToList().First().Name,
                    TenantId = user?.TenantId,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = user?.Email,
                };
                _reviewRepository.Add(newReview);

                var response = _mapper.Map<ReviewResponse>(newReview);
                if (newReview.ProductId.HasValue)
                {
                    var product = _productRepository.FindByAsync(c => c.Id == newReview.ProductId.Value).FirstOrDefault();
                    response.ProductName = product?.Name!;
                }
                result.BuildResult(response, "Review created successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public AppResponse<SearchResponse<ReviewResponse>> Search(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ReviewResponse>>();
            try
            {
                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = _reviewRepository.CountRecordsByPredicate(query);
                var reviews = _reviewRepository.FindByPredicate(query);
                if (request.SortBy != null)
                    reviews = _reviewRepository.addSort(reviews, request.SortBy);
                else
                    reviews = reviews.OrderBy(x => x.Product!.Name);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * pageSize;
                var reviewList = reviews.Skip(startIndex).Take(pageSize);
                var dtoList = _mapper.Map<List<ReviewResponse>>(reviewList);
                var searchResponse = new SearchResponse<ReviewResponse>
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

        private ExpressionStarter<Review> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Review>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "Rating":
                                if (Enum.TryParse<Ratings>(filter.Value.ToString(), out var rating))
                                    predicate = predicate.And(x => x.Rating == rating);
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
