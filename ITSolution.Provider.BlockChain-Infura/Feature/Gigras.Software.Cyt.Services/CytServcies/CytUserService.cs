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
    public class CytUserService : GenericCytService<ITUser>, ICytUserService
    {
        private readonly ICytUserRepository _cytuserRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CytUserService(IHttpContextAccessor httpContextAccessor, ICytUserRepository cytuserRepository) : base(cytuserRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _cytuserRepository = cytuserRepository;
        }

        public async Task<ITUser> FindByUsernameAndPasswordAsync(string username, string password)
        {
            return await _cytuserRepository.FindByUsernameAndPasswordAsync(username, password);
        }

        public async Task SignInUserAsync(ITUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                // You can add other claims as needed
                // You can add additional claims (e.g., roles, permissions)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                // You can add properties like expiration time, sliding expiration, etc.
            };

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                               claimsPrincipal, authProperties);
        }

        // Additional methods specific to CytUser
    }
}