using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicValidationService : IGenericCytService<DynamicValidation>
    {
        // Additional methods for DynamicFormService
    }

    public class DynamicValidationService : GenericCytService<DynamicValidation>, IDynamicValidationService
    {
        private readonly IDynamicValidationRepository _dynamicvalidationRepository;

        public DynamicValidationService(IDynamicValidationRepository dynamicvalidationRepository) : base(dynamicvalidationRepository)
        {
            _dynamicvalidationRepository = dynamicvalidationRepository;
        }

        // Additional methods specific to DynamicForm
    }
}