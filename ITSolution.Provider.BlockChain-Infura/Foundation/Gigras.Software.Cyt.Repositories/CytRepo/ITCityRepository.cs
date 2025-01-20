using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ICytCityRepository : IGenericCytRepository<ITCity>
    {
    }

    public class CytCityRepository : GenericCytRepository<ITCity>, ICytCityRepository
    {
        public CytCityRepository(CytContext context) : base(context)
        {
        }
    }
}