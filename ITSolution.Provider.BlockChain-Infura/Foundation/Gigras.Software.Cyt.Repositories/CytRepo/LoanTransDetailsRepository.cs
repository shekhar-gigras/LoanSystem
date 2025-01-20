using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ILoanTransDetailsRepository : IGenericCytRepository<LoanTransDetails>
    {
    }

    public class LoanTransDetailsRepository : GenericCytRepository<LoanTransDetails>, ILoanTransDetailsRepository
    {
        public LoanTransDetailsRepository(CytContext context) : base(context)
        {
        }
    }
}