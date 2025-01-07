using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ICytUserRepository : IGenericCytRepository<ITUser>
    {
        Task<ITUser> FindByUsernameAndPasswordAsync(string username, string password);
    }

    public class CytUserRepository : GenericCytRepository<ITUser>, ICytUserRepository
    {
        public CytUserRepository(CytContext context) : base(context)
        {
        }

        public async Task<ITUser> FindByUsernameAndPasswordAsync(string username, string password)
        {
            // Query the database to find the user based on username and password
            var user = await _context.DynamicUsers
                                     .Where(X => (X.UserName == username || X.Email == username) && X.Password == password && X.Active!.Value)
                                     .FirstOrDefaultAsync();

            if (user == null)
            {
                // Handle the case where no user is found
                return null;
            }

            // Return the found user
            return user;
        }
    }
}