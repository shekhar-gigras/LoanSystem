using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ICytStateRepository : IGenericCytRepository<ITState>
    {
    }

    public class CytStateRepository : GenericCytRepository<ITState>, ICytStateRepository
    {
        public CytStateRepository(CytContext context) : base(context)
        {
        }
    }
}