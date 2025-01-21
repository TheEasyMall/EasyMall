using EasyMall.Commons.Enums;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using EasyMall.Models.DTOs.Auth.Requests;
using EasyMall.Models.DTOs.Auth.Responses;
using EasyMall.Services.Interfaces;
using MayNghien.Models.Response.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.Services.Implements
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITenantRepository _tenantRepository;

        public AuthenticationService(IConfiguration config, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor contextAccessor, ITenantRepository tenantRepository)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _tenantRepository = tenantRepository;
        }

        public Task<AppResponse<LogInRequest>> GetInforAccount()
        {
            throw new NotImplementedException();
        }

        public Task<AppResponse<LogInResponse>> LogInUser(LogInRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AppResponse<SignUpResponse>> SignUpUser(SignUpRequest request)
        {
            var result = new AppResponse<SignUpResponse>();
            try
            {
                if (await CheckUserExists(request.Email, request.PhoneNumber))
                    return result.BuildError("User already exists.");

                var newTenant = await CreateTenant(request);
                var createUserResult = await CreateUser(request, newTenant.Id);
                if (!createUserResult.Succeeded)
                    return result.BuildError(string.Join(", ", createUserResult.Errors.Select(e => e.Description)));

                var identityUser = await _userManager.FindByEmailAsync(request.Email);
                if (identityUser == null)
                    return result.BuildError("Failed to retrieve created user.");

                await AssignRole(identityUser, "TenantAdmin");
                var response = new SignUpResponse
                {
                    Email = request.Email,
                    Token = GenerateAccessToken(new List<Claim>
                    {
                        new Claim("Email", identityUser.Email!),
                        new Claim(ClaimTypes.Role, "TenanAdmin")
                    }),
                    Type = request.Type,
                };
                return result.BuildResult(response, "User registered successfully!");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + ex.StackTrace);
            }
        }

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                _config["Jwt:Isser"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<bool> CheckUserExists(string email, string phoneNumber)
        {
            var userByEmail = await _userManager.FindByEmailAsync(email);
            if (userByEmail != null) return true;

            var userByPhone = _userManager.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber);
            return userByPhone != null;
        }

        private async Task<Tenant> CreateTenant(SignUpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("SignUp request is invalid.");

            var newTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Type = request.Type,
                CreatedBy = request.Email,
                CreatedOn = DateTime.UtcNow,
            };
            _tenantRepository.Add(newTenant);
            return newTenant; 
        }

        private async Task<IdentityResult> CreateUser(SignUpRequest request, Guid tenantId)
        {
            if (request == null)
                throw new ArgumentNullException("SignUp request is invalid.");

            var indentityUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                TenantId = tenantId,
                SecurityStamp = request.Password,
                EmailConfirmed = true,
            };
            return await _userManager.CreateAsync(indentityUser, request.Password);
        }

        private async Task AssignRole(ApplicationUser user, string roleName)
        {
            if (user == null || string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("User of RoleName is invalid.");
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                    throw new Exception("Cannot create role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));

            }

            var assignResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!assignResult.Succeeded)
                throw new Exception("Cannot assign permission role to User: " + string.Join(", ", assignResult.Errors.Select(e => e.Description)));
        }
    }
}
