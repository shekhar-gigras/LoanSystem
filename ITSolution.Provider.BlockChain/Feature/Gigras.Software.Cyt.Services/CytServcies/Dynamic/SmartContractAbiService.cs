using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface ISmartContractAbiService : IGenericCytService<SmartContractAbi>
    {
        // Additional methods for DynamicFormService
        Task<SmartContractAbi> SubmitData(Dictionary<string, string> fieldValues);

        Task<SmartContractAbi> GetData(int id);

        Task<SmartContractAbi> GetEditData(int id);

        Task<List<SmartContractAbi>> GetList();
    }

    public class SmartContractAbiService : GenericCytService<SmartContractAbi>, ISmartContractAbiService
    {
        private readonly ISmartContractAbiRepository _SmartContractAbiRepository;
        private readonly ICytAdminService _cytAdminService;

        public SmartContractAbiService(ISmartContractAbiRepository SmartContractAbiRepository, ICytAdminService cytAdminService) : base(SmartContractAbiRepository)
        {
            _SmartContractAbiRepository = SmartContractAbiRepository;
            _cytAdminService = cytAdminService;
        }

        public async Task<List<SmartContractAbi>> GetList()
        {
            var data = await _SmartContractAbiRepository.GetAllAsync(x => !x.IsDelete);
            return data;
        }

        public async Task<SmartContractAbi> SubmitData(Dictionary<string, string> fieldValues)
        {
            int id = Convert.ToInt32(fieldValues["Id"].ToString());

            return await Add(fieldValues);
        }

        private async Task<SmartContractAbi> Add(Dictionary<string, string> fieldValues)
        {
            foreach (var obj in await _SmartContractAbiRepository.GetAllAsync(x => !x.IsDelete && x.IsActive))
            {
                obj.IsActive = false;
                obj.IsDelete = true;
            }
            var lookup = new SmartContractAbi()
            {
                Abi = fieldValues["Abi"].ToString(),
                CreatedBy = await _cytAdminService.GetUserName(),
                CreatedAt = DateTime.Now,
                UpdatedBy = await _cytAdminService.GetUserName(),
                UpdatedAt = DateTime.Now,
                IsActive = true,
                IsDelete = false
            };
            return await _SmartContractAbiRepository.AddAsync(lookup);
        }

        public async Task<SmartContractAbi> GetData(int id)
        {
            var data = await _SmartContractAbiRepository.GetByIdAsync(id, x => x.IsActive && !x.IsDelete);
            return data;
        }

        public async Task<SmartContractAbi> GetEditData(int id)
        {
            var data = await GetData(id);
            return data;
        }

        // Additional methods specific to DynamicForm
    }
}