using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicFormSectionService : IGenericCytService<FormsSection>
    {
        // Additional methods for DynamicFormService
    }

    public class DynamicFormSectionService : GenericCytService<FormsSection>, IDynamicFormSectionService
    {
        private readonly IDynamicFormSectionRepository _dynamicformsectionRepository;

        public DynamicFormSectionService(IDynamicFormSectionRepository dynamicformSectionRepository) : base(dynamicformSectionRepository)
        {
            _dynamicformsectionRepository = dynamicformSectionRepository;
        }

        // Additional methods specific to DynamicForm
    }
}