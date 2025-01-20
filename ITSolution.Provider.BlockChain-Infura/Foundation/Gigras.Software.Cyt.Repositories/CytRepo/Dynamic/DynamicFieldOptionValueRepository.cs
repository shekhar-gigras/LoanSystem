using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicFieldOptionValueRepository : IGenericCytRepository<FieldOptionValue>
    {
    }

    public class DynamicFieldOptionValueRepository : GenericCytRepository<FieldOptionValue>, IDynamicFieldOptionValueRepository
    {
        public DynamicFieldOptionValueRepository(CytContext context) : base(context)
        {
        }
    }
}