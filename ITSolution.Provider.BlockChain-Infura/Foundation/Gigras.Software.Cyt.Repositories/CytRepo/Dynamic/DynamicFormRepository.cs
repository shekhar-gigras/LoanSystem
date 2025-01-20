using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicFormRepository : IGenericCytRepository<Form>
    {
    }

    public class DynamicFormRepository : GenericCytRepository<Form>, IDynamicFormRepository
    {
        public DynamicFormRepository(CytContext context) : base(context)
        {
        }
    }
}