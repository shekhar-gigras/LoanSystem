using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace Gigras.Software.BlockChain.Service.Model
{
    [FunctionOutput]
    public class LoanDetails : CommonAndInterestDetails
    {
        public string? LoanId { get; set; }
    }

    [FunctionOutput]
    public class CommonAndInterestDetails
    {
        [Parameter("string", "emiPaymentStartDate", 1)]
        public string EmiPaymentStartDate { get; set; }

        [Parameter("uint256", "principalAmount", 2)]
        public BigInteger PrincipalAmount { get; set; }

        [Parameter("uint256", "fixedInterestRate", 3)]
        public BigInteger FixedInterestRate { get; set; }

        [Parameter("uint256", "monthlyPaymentAmount", 4)]
        public BigInteger MonthlyPaymentAmount { get; set; }

        [Parameter("string", "maturityDate", 5)]
        public string MaturityDate { get; set; }

        [Parameter("uint8", "emiPaymentDay", 6)]
        public byte EmiPaymentDay { get; set; }

        [Parameter("string", "interestRateChangeDate", 7)]
        public string InterestRateChangeDate { get; set; }

        [Parameter("string", "lenderName", 8)]
        public string LenderName { get; set; }

        [Parameter("string", "borrowerName", 9)]
        public string BorrowerName { get; set; }

        public CommonAndInterestDetails()
        {
        }
    }
}