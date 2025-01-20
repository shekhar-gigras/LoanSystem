using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicFormFieldService : IGenericCytService<FormField>
    {
        // Additional methods for DynamicFormService
    }

    public class DynamicFormFieldService : GenericCytService<FormField>, IDynamicFormFieldService
    {
        private readonly IDynamicFormFieldRepository _dynamicFormFieldRepository;

        public DynamicFormFieldService(IDynamicFormFieldRepository dynamicFormFieldRepository) : base(dynamicFormFieldRepository)
        {
            _dynamicFormFieldRepository = dynamicFormFieldRepository;
        }

        // Additional methods specific to DynamicForm
    }
}