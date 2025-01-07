using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicCtrlRepository : IGenericCytRepository<DynamicCtrl>
    {
    }

    public class DynamicCtrlRepository : GenericCytRepository<DynamicCtrl>, IDynamicCtrlRepository
    {
        public DynamicCtrlRepository(CytContext context) : base(context)
        {
        }
    }
}