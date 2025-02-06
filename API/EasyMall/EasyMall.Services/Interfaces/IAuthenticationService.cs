using EasyMall.DTOs.DTOs.Auth.Requests;
using EasyMall.DTOs.DTOs.Auth.Responses;
using MayNghien.Models.Response.Base;

namespace EasyMall.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AppResponse<LogInResponse>> LogInUser(LogInRequest request);
        Task<AppResponse<LogInRequest>> GetInforAccount();
        Task<AppResponse<SignUpResponse>> SignUpUser(SignUpRequest request);
    }
}
