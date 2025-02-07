using System.Numerics;

namespace Gigras.Software.BlockChain.Service.Model
{
    public class BorrowerLoanRequest
    {
        public BigInteger LoanAmount { get; set; }
        public BigInteger InterestRate { get; set; }
        public BigInteger RepaymentDurationInMonths { get; set; }
    }
}