using Gigras.Software.Cyt.Repositories.CytRepo;
using Gigras.Software.Cyt.Services.ICytServices;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.Services;
using System.Linq.Expressions;

namespace Gigras.Software.Cyt.Services.CytServcies
{
    public interface ILoanBuyInterestService : IGenericCytService<LoanBuyInterest>
    {
        Task<List<LoanBuyInterest>> GetList();
    }

    public class LoanBuyInterestService : GenericCytService<LoanBuyInterest>, ILoanBuyInterestService
    {
        private readonly ILoanBuyInterestRepository _LoanBuyInterestRepository;
        private readonly ICytAdminService _cytAdminService;

        public LoanBuyInterestService(ILoanBuyInterestRepository LoanBuyInterestRepository, ICytAdminService cytAdminService) : base(LoanBuyInterestRepository)
        {
            _LoanBuyInterestRepository = LoanBuyInterestRepository;
            _cytAdminService = cytAdminService;
        }

        public async Task<List<LoanBuyInterest>> GetList()
        {
            var userdetail = await _cytAdminService.GetUserDetails();
            var lenderid = await _cytAdminService.GetUserUniqueId();
            var data = await _LoanBuyInterestRepository.GetAllAsync(x =>
                x.IsActive
                && !x.IsDelete
                && (x.SellerId == Guid.Parse(lenderid!) || userdetail.Roles.Contains("Admin"))
                && !x.IsClosedSellerLoan
                && !x.IsDenied,
                 new Expression<Func<LoanBuyInterest, object?>>[] { x => x.LoanDetails }
            );
            foreach(var item in data)
            {
                var user = await _cytAdminService.GetByIdAsync(0, x => x.UserId == item.BuyerId);
                item.LendderEmail = user!.Email;
                item.LendderPhone = user!.Phone;
                item.LendderName = user.Name;
            }
            data = data.OrderBy(x => x.LoanId).ToList();
            return data.ToList();
        }
        // Additional methods specific to DynamicForm
    }
}