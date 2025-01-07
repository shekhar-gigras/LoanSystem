using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.ICytServices
{
    public interface ICytUserService : IGenericCytService<ITUser>
    {
        Task<ITUser> FindByUsernameAndPasswordAsync(string username, string password);

        Task SignInUserAsync(ITUser user);

        // Additional methods for CytUserService
    }
}