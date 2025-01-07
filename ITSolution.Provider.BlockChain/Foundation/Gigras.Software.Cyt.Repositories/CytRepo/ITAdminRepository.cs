using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.General.Helper;
using Gigras.Software.Generic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ICytAdminRepository : IGenericCytRepository<ITAdmin>
    {
        Task<ITAdmin> FindByUsernameAndPasswordAsync(string username, string password);
    }

    public class CytAdminRepository : GenericCytRepository<ITAdmin>, ICytAdminRepository
    {
        public CytAdminRepository(CytContext context) : base(context)
        {
        }

        public async Task<ITAdmin> FindByUsernameAndPasswordAsync(string username, string password)
        {
            string hashpassword = PasswordHelper.HashPassword(password);
            // Query the database to find the user based on username and password
            var user = await _context.DynamicAdmins
                                     .Where(a => (a.UserName == username || a.Email == username) && a.Password == hashpassword)
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