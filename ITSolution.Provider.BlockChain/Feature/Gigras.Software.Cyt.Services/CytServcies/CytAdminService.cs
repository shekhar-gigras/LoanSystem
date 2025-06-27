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

        public async Task<string?> GetLenderName()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.Identity?.IsAuthenticated == true
                ? user.Claims.FirstOrDefault(c => c.Type == "LenderName")?.Value
                : null;
        }

        public async Task<string?> GetUserUniqueId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.Identity?.IsAuthenticated == true
                ? user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value
                : null;
        }

        public async Task<bool> IsAddLoanAccess()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var claim = user.Claims.FirstOrDefault(c => c.Type == "IsAddLoan")?.Value;

            return bool.TryParse(claim, out var result) && result;
        }

        public async Task<bool> IsEditLoanAccess()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var claim = user.Claims.FirstOrDefault(c => c.Type == "IsEditLoan")?.Value;

            return bool.TryParse(claim, out var result) && result;
        }

        public async Task<bool> IsDeleteLoanAccess()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var claim = user.Claims.FirstOrDefault(c => c.Type == "IsDeleteLoan")?.Value;

            return bool.TryParse(claim, out var result) && result;
        }

        public async Task<bool> IsAdmin()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var userdetail = await this.GetUserDetails();

            return userdetail.Roles.Contains("Admin");
        }

        public async Task<bool> IsVisibleSaleLoanAccess()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var claim = user.Claims.FirstOrDefault(c => c.Type == "IsVisibleLoanSale")?.Value;

            return bool.TryParse(claim, out var result) && result;
        }

        public async Task<List<ITAdmin>> GetUserList(string q = "")
        {
            var data = new List<ITAdmin>();
            var allData = await _cytadminRepository.GetAllAsync(x => x.UserName!.ToLower() != "admin" && !x.IsDelete && x.IsConfirmLink);

            if (!string.IsNullOrEmpty(q))
            {
                // Try to parse the user input as a DateTime (DDMMYYYY format)
                DateTime? parsedDate = null;
                if (DateTime.TryParseExact(q, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out var dateResult))
                {
                    parsedDate = dateResult;
                }
                data = allData
                    .Where(item =>
                    {
                        return item.GetType().GetProperties()
                            .Where(prop => prop.PropertyType == typeof(string) || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(DateTime))
                            .Any(prop =>
                            {
                                var value = prop.GetValue(item);

                                if (value is string stringValue)
                                {
                                    // Handle string fields (case-insensitive)
                                    return stringValue.Contains(q, StringComparison.OrdinalIgnoreCase);
                                }
                                else if (value is decimal decimalValue)
                                {
                                    // Handle decimal fields (convert the query to decimal if possible)
                                    if (decimal.TryParse(q, out var decimalQuery))
                                    {
                                        return decimalValue.ToString().Contains(decimalQuery.ToString());
                                    }
                                    return false;
                                }
                                else if (value is DateTime dateValue)
                                {
                                    // Handle DateTime fields (compare date parts only, without time)
                                    return parsedDate.HasValue && Convert.ToInt32(dateValue.ToString("yyyyMMdd")) == Convert.ToInt32(parsedDate.Value.ToString("yyyyMMdd"));
                                }

                                return false;
                            });
                    })
                    .ToList();
            }
            else
            {
                data = allData.ToList();
            }
            data = data.OrderBy(x => x.Name).ToList();
            return data;
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
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role!),
                 new Claim("UserId", $"{user.UserId}"),
                 new Claim("IsActive", $"{user.IsActive}"),
                 new Claim("IsDelete", $"{user.IsDelete}"),
                 new Claim("IsBlock", $"{user.IsBlock}"),
                 new Claim("IsAddLoan", $"{user.IsAddLoan}"),
                 new Claim("IsEditLoan", $"{user.IsEditLoan}"),
                 new Claim("IsDeleteLoan", $"{user.IsDeleteLoan}"),
                 new Claim("IsVisibleLoanSale", $"{user.IsVisibleLoanSale}"),
                 new Claim("IsConfirmLink", $"{user.IsConfirmLink}"),
                 new Claim("LastLogin", $"{user.LastLogin}"),
                  new Claim("LenderName", $"{user.Name}"),
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

        public async Task<ITAdmin> FindByUsernameAsync(string username)
        {
            return await _cytadminRepository.FindByUsernameAsync(username);
        }
    }
}