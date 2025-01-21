using EasyMall.Models.DTOs.Auth.Requests;
using EasyMall.Models.DTOs.Auth.Responses;
using MayNghien.Models.Response.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AppResponse<LogInResponse>> LogInUser(LogInRequest request);
        Task<AppResponse<LogInRequest>> GetInforAccount();
        Task<AppResponse<SignUpResponse>> SignUpUser(SignUpRequest request);
    }
}
