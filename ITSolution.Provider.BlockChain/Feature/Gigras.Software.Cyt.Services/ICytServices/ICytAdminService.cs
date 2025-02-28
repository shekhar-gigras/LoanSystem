using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.ICytServices
{
    public interface ICytAdminService : IGenericCytService<ITAdmin>
    {
        Task<ITAdmin> FindByUsernameAndPasswordAsync(string username, string password);

        Task SignInUserAsync(ITAdmin user);

        Task<string?> GetUserName();

        Task<string?> GetUserEmail();

        Task<string?> GetUserId();

        Task<List<ITAdmin>> GetUserList(string q = "");

        Task<(string? UserId, List<string> Roles)> GetUserDetails();

        Task<bool> IsAddLoanAccess();

        Task<bool> IsVisibleSaleLoanAccess();

        Task<bool> IsEditLoanAccess();

        Task<bool> IsDeleteLoanAccess();

        Task<string?> GetUserUniqueId();

        Task<bool> IsAdmin();

        Task<string?> GetLenderName();

        // Additional methods for CytAdminService
    }
}