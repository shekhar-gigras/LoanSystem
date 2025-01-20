using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies.Dynamic
{
    public interface IDynamicFieldTypeValidaionService : IGenericCytService<FieldTypeValidation>
    {
    }

    public class DynamicFieldTypeValidaionService : GenericCytService<FieldTypeValidation>, IDynamicFieldTypeValidaionService
    {
        private readonly IDynamicFieldTypeValidaionRepository _dynamicFieldTypeValidaionRepository;

        public DynamicFieldTypeValidaionService(IDynamicFieldTypeValidaionRepository dynamicFieldTypeValidaionRepository) : base(dynamicFieldTypeValidaionRepository)
        {
            _dynamicFieldTypeValidaionRepository = dynamicFieldTypeValidaionRepository;
        }
    }
}