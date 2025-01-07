using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicBlockChainRepository : IGenericCytRepository<DynamicBlockChainField>
    {
    }

    public class DynamicBlockChainRepository : GenericCytRepository<DynamicBlockChainField>, IDynamicBlockChainRepository
    {
        public DynamicBlockChainRepository(CytContext context) : base(context)
        {
        }
    }
}