using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.General.Helper;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface ILoanDetailsService : IGenericCytService<LoanDetails>
    {
        // Additional methods for DynamicFormService
        Task<LoanDetails> SubmitData(Dictionary<string, string> fieldValues);

        Task<List<LoanDetails>> GetList();
        Task<LoanDetails> GetData(int id);
        Task<LoanDetails> GetEditData(int id);
    }

    public class LoanDetailsService : GenericCytService<LoanDetails>, ILoanDetailsService
    {
        private readonly ILoanDetailsRepository _LoanDetailsRepository;
        private readonly ICytAdminService _cytAdminService;

        public LoanDetailsService(ILoanDetailsRepository LoanDetailsRepository, ICytAdminService cytAdminService) : base(LoanDetailsRepository)
        {
            _LoanDetailsRepository = LoanDetailsRepository;
            _cytAdminService = cytAdminService;
        }

        public async Task<LoanDetails> SubmitData(Dictionary<string, string> fieldValues)
        {
            string loanid = fieldValues["LoanId"].ToString();

            return await Add(loanid, fieldValues);
        }

        private async Task<LoanDetails> Add(string loanid, Dictionary<string, string> fieldValues)
        {
            var objList = await _LoanDetailsRepository.GetAllAsync(x => x.LoanId.ToString().ToLower() == loanid.ToLower());
            if (objList.Count == 0)
            {
                var obj = new LoanDetails();
                ObjectPopulator.PopulateObject<LoanDetails>(obj, fieldValues);
                obj.CreatedAt = DateTime.Now;
                obj.CreatedBy = await _cytAdminService.GetUserName();
                obj.UpdatedBy = await _cytAdminService.GetUserName();
                obj.UpdatedAt = DateTime.Now;
                obj.IsActive = true;
                obj.IsDelete = false;
                return await _LoanDetailsRepository.AddAsync(obj);
            }
            else
            {
                var obj = objList.FirstOrDefault();
                ObjectPopulator.PopulateObject<LoanDetails>(obj, fieldValues);
                obj.UpdatedBy = await _cytAdminService.GetUserName();
                obj.UpdatedAt = DateTime.Now;
                await _LoanDetailsRepository.UpdateAsync(obj);
                return obj;
            }
        }

        public async Task<List<LoanDetails>> GetList()
        {
            var data = await _LoanDetailsRepository.GetAllAsync();
            return data.ToList();
        }

        public async Task<LoanDetails> GetData(int id)
        {
            var data = await _LoanDetailsRepository.GetByIdAsync(id);
            return data;
        }

        public async Task<LoanDetails> GetEditData(int id)
        {
            var data = await GetData(id);
            return data;
        }
        // Additional methods specific to DynamicForm
    }
}