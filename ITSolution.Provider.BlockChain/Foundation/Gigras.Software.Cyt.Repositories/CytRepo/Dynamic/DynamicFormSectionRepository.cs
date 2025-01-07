using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IDynamicFormSectionRepository : IGenericCytRepository<FormsSection>
    {
    }

    public class DynamicFormSectionRepository : GenericCytRepository<FormsSection>, IDynamicFormSectionRepository
    {
        public DynamicFormSectionRepository(CytContext context) : base(context)
        {
        }
    }
}