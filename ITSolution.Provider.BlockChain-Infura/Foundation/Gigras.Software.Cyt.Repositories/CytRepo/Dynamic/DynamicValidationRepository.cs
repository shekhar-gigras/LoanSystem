using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicValidationRepository : IGenericCytRepository<DynamicValidation>
    {
    }

    public class DynamicValidationRepository : GenericCytRepository<DynamicValidation>, IDynamicValidationRepository
    {
        public DynamicValidationRepository(CytContext context) : base(context)
        {
        }
    }
}