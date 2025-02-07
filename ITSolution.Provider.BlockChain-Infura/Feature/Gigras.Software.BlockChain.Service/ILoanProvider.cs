using Gigras.Software.BlockChain.Service.Model;

namespace Gigras.Software.BlockChain.Service
{
    public interface ILoanProvider
    {
        Task<string> GetSmartContractOwnerAddress();

        Task<int> GetSmartContractBalance();

        Task<List<string>> GetLoanIds();

        Task<decimal> GetLenderBalance(string lenderAddress);

        Task<string> FundContractAsync(string lenderAddress, int amountInEther);

        Task<string> TakeOutContractFundsAsync(string lenderAddress, decimal amountToWithdraw);

        Task<string> ChangeLenderAsync(string lenderAddress, string newLenderAddress);

        Task<string> SetCommonAndInterestDetailsAsync(string lenderAddress, LoanDetails loanDetails);

        Task<string> SetPaymentAndLateDetailsAsync(string lenderAddress, InterestDetails details);

        Task<string> SetAdjustableInterestRateDetailsAsync(string lenderAddress, OtherDetails details);

        Task<Loan> GetLoanDetailsAsync(string loanId);

        Task<string> DeleteLoanAsync(string lenderAddress, string loanId);
    }
}