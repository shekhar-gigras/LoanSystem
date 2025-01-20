using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface IBorrowerLoanRepository : IGenericCytRepository<BorrowerLoan>
    {
    }

    public class BorrowerLoanRepository : GenericCytRepository<BorrowerLoan>, IBorrowerLoanRepository
    {
        public BorrowerLoanRepository(CytContext context) : base(context)
        {
        }
    }
}