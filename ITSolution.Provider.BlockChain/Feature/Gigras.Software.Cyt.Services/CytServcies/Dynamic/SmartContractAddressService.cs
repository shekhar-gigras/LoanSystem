using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface ISmartContractAddressService : IGenericCytService<SmartContractAddress>
    {
        // Additional methods for DynamicFormService
        Task<SmartContractAddress> SubmitData(Dictionary<string, string> fieldValues);

        Task<SmartContractAddress> GetData(int id);

        Task<SmartContractAddress> GetEditData(int id);

        Task<List<SmartContractAddress>> GetList();
    }

    public class SmartContractAddressService : GenericCytService<SmartContractAddress>, ISmartContractAddressService
    {
        private readonly ISmartContractAddressRepository _SmartContractAddressRepository;
        private readonly ICytAdminService _cytAdminService;

        public SmartContractAddressService(ISmartContractAddressRepository SmartContractAddressRepository, ICytAdminService cytAdminService) : base(SmartContractAddressRepository)
        {
            _SmartContractAddressRepository = SmartContractAddressRepository;
            _cytAdminService = cytAdminService;
        }

        public async Task<List<SmartContractAddress>> GetList()
        {
            var data = await _SmartContractAddressRepository.GetAllAsync();
            return data.ToList();
        }

        public async Task<SmartContractAddress> SubmitData(Dictionary<string, string> fieldValues)
        {
            int id = Convert.ToInt32(fieldValues["Id"].ToString());

            return await Add(fieldValues);
        }

        private async Task<SmartContractAddress> Add(Dictionary<string, string> fieldValues)
        {
            var IsLender = Convert.ToBoolean(fieldValues["IsLender"].ToString());
            foreach (var obj in await _SmartContractAddressRepository.GetAllAsync(x => !x.IsDelete && x.IsActive))
            {
                obj.IsActive = false;
                obj.IsDelete = false;
            }
            var lookup = new SmartContractAddress()
            {
                Version = fieldValues["Version"].ToString(),
                IsLender = IsLender,
                ContractAddress = fieldValues["ContractAddress"].ToString(),
                CreatedBy = await _cytAdminService.GetUserName(),
                CreatedAt = DateTime.Now,
                UpdatedBy = await _cytAdminService.GetUserName(),
                UpdatedAt = DateTime.Now,
                IsActive = true,
                IsDelete = false
            };
            return await _SmartContractAddressRepository.AddAsync(lookup);
        }

        public async Task<SmartContractAddress> GetData(int id)
        {
            var data = await _SmartContractAddressRepository.GetByIdAsync(id);
            return data;
        }

        public async Task<SmartContractAddress> GetEditData(int id)
        {
            var data = await GetData(id);
            return data;
        }

        // Additional methods specific to DynamicForm
    }
}