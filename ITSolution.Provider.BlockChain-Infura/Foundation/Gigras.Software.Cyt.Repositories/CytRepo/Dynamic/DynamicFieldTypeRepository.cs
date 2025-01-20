using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicFieldTypeRepository : IGenericCytRepository<FieldType>
    {
    }

    public class DynamicFieldTypeRepository : GenericCytRepository<FieldType>, IDynamicFieldTypeRepository
    {
        public DynamicFieldTypeRepository(CytContext context) : base(context)
        {
        }
    }
}