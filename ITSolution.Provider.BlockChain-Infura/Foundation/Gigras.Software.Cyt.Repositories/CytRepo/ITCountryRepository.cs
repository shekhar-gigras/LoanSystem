using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ICytCountryRepository : IGenericCytRepository<ITCountry>
    {
    }

    public class CytCountryRepository : GenericCytRepository<ITCountry>, ICytCountryRepository
    {
        public CytCountryRepository(CytContext context) : base(context)
        {
        }
    }
}