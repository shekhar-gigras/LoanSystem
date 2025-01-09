using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ILoanDetailsRepository : IGenericCytRepository<LoanDetails>
    {
    }

    public class LoanDetailsRepository : GenericCytRepository<LoanDetails>, ILoanDetailsRepository
    {
        public LoanDetailsRepository(CytContext context) : base(context)
        {
        }
    }
}