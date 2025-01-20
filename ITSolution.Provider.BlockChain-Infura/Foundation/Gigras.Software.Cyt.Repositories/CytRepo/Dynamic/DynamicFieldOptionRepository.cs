using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicFieldOptionRepository : IGenericCytRepository<FieldOption>
    {
    }

    public class DynamicFieldOptionRepository : GenericCytRepository<FieldOption>, IDynamicFieldOptionRepository
    {
        public DynamicFieldOptionRepository(CytContext context) : base(context)
        {
        }
    }
}