using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IDynamicBlockChainService : IGenericCytService<DynamicBlockChainField>
    {
        // Additional methods for DynamicFormService
    }

    public class DynamicBlockChainService : GenericCytService<DynamicBlockChainField>, IDynamicBlockChainService
    {
        private readonly IDynamicBlockChainRepository _dynamicblockchainRepository;

        public DynamicBlockChainService(IDynamicBlockChainRepository dynamicblockchainRepository) : base(dynamicblockchainRepository)
        {
            _dynamicblockchainRepository = dynamicblockchainRepository;
        }

        // Additional methods specific to DynamicForm
    }
}