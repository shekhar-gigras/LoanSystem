using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicFormFieldRepository : IGenericCytRepository<FormField>
    {
    }

    public class DynamicFormFieldRepository : GenericCytRepository<FormField>, IDynamicFormFieldRepository
    {
        public DynamicFormFieldRepository(CytContext context) : base(context)
        {
        }
    }
}