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
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static MayNghien.Infrastructure.CommonMessage.AuthResponseMessage;

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

        public async Task<AppResponse<LogInRequest>> GetInforAccount()
        {
            var result = new AppResponse<LogInRequest>();
            try
            {
                var user = await FindUserByEmailAsync(_contextAccessor.HttpContext.User.Identity.Name);
                var userDto = new LogInRequest();
                userDto.Email = user.Email!;
                Log.Information(user.Email + " login");
                result.BuildResult(userDto);
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        public async Task<AppResponse<LogInResponse>> LogInUser(LogInRequest request)
        {
            var result = new AppResponse<LogInResponse>();
            try
            {
                var identityUser = await FindUserByEmailAsync(request.Email);
                if (identityUser == null)
                {
                    if (request.Email == "admin@gmail.com")
                    {
                        var newIdentity = await CreateAdminUserAsync(request.Email);
                        return await LogInUser(request);
                    }
                    return result.BuildError(ERR_MSG_UserNotFound);
                }
                if (!await _userManager.CheckPasswordAsync(identityUser, request.Password))
                {
                    return result.BuildError("Invalid credentials.");
                }

                var roles = await _userManager.GetRolesAsync(identityUser);
                if (roles == null || !roles.Any())
                {
                    return result.BuildError("User has no role assigned.");
                }

                var claims = await GetClaims(request, identityUser);
                var tokenString = GenerateAccessToken(claims);
                var loginResponse = new LogInResponse()
                {
                    Email = identityUser.Email,
                    Role = roles.FirstOrDefault(),
                    Token = tokenString,
                };
                return result.BuildResult(loginResponse);
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
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
                        new Claim(ClaimTypes.Email, identityUser.Email!),
                        new Claim(ClaimTypes.Role, "TenantAdmin")
                    }),
                    Type = request.Type,
                };
                return result.BuildResult(response, "User registered successfully!");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        private async Task<ApplicationUser> FindUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private async Task<ApplicationUser> CreateAdminUserAsync(string email)
        {
            var newIdentity = new ApplicationUser
            {
                Email = email,
                EmailConfirmed = true,
                UserName = email,
            };
            await _userManager.CreateAsync(newIdentity);
            await _userManager.AddPasswordAsync(newIdentity, "Abc@123");

            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
            {
                var role = new IdentityRole { Name = "SuperAdmin" };
                await _roleManager.CreateAsync(role);
            }
            await _userManager.AddToRoleAsync(newIdentity, "SuperAdmin");
            return newIdentity;
        }

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<bool> CheckUserExists(string email, string phoneNumber)
        {
            var userByEmail = await FindUserByEmailAsync(email);
            if (userByEmail != null) return true;

            var userByPhone = _userManager.Users.FirstOrDefault(p => p.PhoneNumber == phoneNumber);
            return userByPhone != null;
        }

        private async Task<Tenant> CreateTenant(SignUpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "SignUp request is invalid.");

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
                throw new ArgumentNullException(nameof(request), "SignUp request is invalid.");

            var identityUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                TenantId = tenantId,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                PhoneNumber = request.PhoneNumber
            };
            return await _userManager.CreateAsync(identityUser, request.Password);
        }

        private async Task AssignRole(ApplicationUser user, string roleName)
        {
            if (user == null || string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException("User or RoleName is invalid.");
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

        private async Task<List<Claim>> GetClaims(LogInRequest user, ApplicationUser identityUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(identityUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim(ClaimTypes.Name, user.Email));
            }
            return claims;
        }
    }
}
