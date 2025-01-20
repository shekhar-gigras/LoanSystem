using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicFieldValidationRepository : IGenericCytRepository<FieldValidation>
    {
    }

    public class DynamicFieldValidationRepository : GenericCytRepository<FieldValidation>, IDynamicFieldValidationRepository
    {
        public DynamicFieldValidationRepository(CytContext context) : base(context)
        {
        }
    }
}