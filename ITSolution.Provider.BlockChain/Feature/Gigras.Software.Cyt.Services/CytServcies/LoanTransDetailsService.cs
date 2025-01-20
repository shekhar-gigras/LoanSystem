using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface ILoanTransDetailsService : IGenericCytService<LoanTransDetails>
    {
    }

    public class LoanTransDetailsService : GenericCytService<LoanTransDetails>, ILoanTransDetailsService
    {
        private readonly ILoanTransDetailsRepository _LoanTransDetailsRepository;
        private readonly ICytAdminService _cytAdminService;

        public LoanTransDetailsService(ILoanTransDetailsRepository LoanTransDetailsRepository, ICytAdminService cytAdminService) : base(LoanTransDetailsRepository)
        {
            _LoanTransDetailsRepository = LoanTransDetailsRepository;
            _cytAdminService = cytAdminService;
        }

        // Additional methods specific to DynamicForm
    }
}