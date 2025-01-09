using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface IBorrowerLoanService : IGenericCytService<BorrowerLoan>
    {
        // Additional methods for DynamicFormService
    }

    public class BorrowerLoanService : GenericCytService<BorrowerLoan>, IBorrowerLoanService
    {
        private readonly IBorrowerLoanRepository _BorrowerLoanRepository;

        public BorrowerLoanService(IBorrowerLoanRepository BorrowerLoanRepository) : base(BorrowerLoanRepository)
        {
            _BorrowerLoanRepository = BorrowerLoanRepository;
        }

        // Additional methods specific to DynamicForm
    }
}