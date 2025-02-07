using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Repositories;

namespace Gigras.Software.Cyt.Repositories.CytRepo
{
    public interface ILoanBuyInterestRepository : IGenericCytRepository<LoanBuyInterest>
    {
    }

    public class LoanBuyInterestRepository : GenericCytRepository<LoanBuyInterest>, ILoanBuyInterestRepository
    {
        public LoanBuyInterestRepository(CytContext context) : base(context)
        {
        }
    }
}