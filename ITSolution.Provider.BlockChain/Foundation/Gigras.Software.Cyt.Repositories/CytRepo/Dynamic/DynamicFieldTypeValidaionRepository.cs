using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicFieldTypeValidaionRepository : IGenericCytRepository<FieldTypeValidation>
    {
    }

    public class DynamicFieldTypeValidaionRepository : GenericCytRepository<FieldTypeValidation>, IDynamicFieldTypeValidaionRepository
    {
        public DynamicFieldTypeValidaionRepository(CytContext context) : base(context)
        {
        }
    }
}