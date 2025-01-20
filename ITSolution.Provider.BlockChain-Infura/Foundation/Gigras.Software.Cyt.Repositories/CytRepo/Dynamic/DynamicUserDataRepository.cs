using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicUserDataRepository : IGenericCytRepository<DynamicUserData>
    {
    }

    public class DynamicUserDataRepository : GenericCytRepository<DynamicUserData>, IDynamicUserDataRepository
    {
        public DynamicUserDataRepository(CytContext context) : base(context)
        {
        }
    }
}