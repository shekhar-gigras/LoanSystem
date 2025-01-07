using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicCtrlService : IGenericCytService<DynamicCtrl>
    {
        // Additional methods for DynamicFormService
    }

    public class DynamicCtrlService : GenericCytService<DynamicCtrl>, IDynamicCtrlService
    {
        private readonly IDynamicCtrlRepository _dynamicctrlRepository;

        public DynamicCtrlService(IDynamicCtrlRepository dynamicctrlRepository) : base(dynamicctrlRepository)
        {
            _dynamicctrlRepository = dynamicctrlRepository;
        }

        // Additional methods specific to DynamicForm
    }
}