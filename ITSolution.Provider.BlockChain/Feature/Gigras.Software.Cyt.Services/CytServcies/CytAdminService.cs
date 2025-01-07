using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Gigras.Software.Cyt.Services.CytService
{
    public class CytAdminService : GenericCytService<ITAdmin>, ICytAdminService
    {
        private readonly ICytAdminRepository _cytadminRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CytAdminService(IHttpContextAccessor httpContextAccessor, ICytAdminRepository cytadminRepository) : base(cytadminRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _cytadminRepository = cytadminRepository;
        }

        public async Task<string?> GetUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.Identity?.IsAuthenticated == true ? user.Identity.Name : null;
        }

        public async Task<string?> GetUserEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        }

        public async Task<(string? UserId, List<string> Roles)> GetUserDetails()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var roles = user.Claims
                                .Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList();

                return (userId, roles);
            }

            return (null, new List<string>());
        }

        public async Task<string?> GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.Identity?.IsAuthenticated == true
                ? user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                : null;
        }

        public async Task<ITAdmin> FindByUsernameAndPasswordAsync(string username, string password)
        {
            return await _cytadminRepository.FindByUsernameAndPasswordAsync(username, password);
        }

        public async Task SignInUserAsync(ITAdmin user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!), // Example of adding email claim
                new Claim(ClaimTypes.Role, user.UserName == "admin"?"Admin":"User"), // Adding a role (e.g., "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                // Set the expiration time for the cookie (e.g., 1 hour)
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                               claimsPrincipal, authProperties);
        }

        // Additional methods specific to CytAdmin
    }
}