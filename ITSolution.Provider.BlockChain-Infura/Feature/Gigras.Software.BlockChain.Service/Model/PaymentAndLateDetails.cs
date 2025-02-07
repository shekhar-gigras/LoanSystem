using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace Gigras.Software.BlockChain.Service.Model
{
    [FunctionOutput]
    public class InterestDetails : PaymentAndLateDetails
    {
        public string? LoanId { get; set; }
    }

    [FunctionOutput]
    public class PaymentAndLateDetails
    {
        [Parameter("uint8", "latePaymentGracePeriod", 1)]
        public byte LatePaymentGracePeriod { get; set; }

        [Parameter("uint256", "lateChargePercentage", 2)]
        public BigInteger LateChargePercentage { get; set; }

        [Parameter("uint256", "margin", 3)]
        public BigInteger Margin { get; set; }

        [Parameter("uint256", "currentIndex", 4)]
        public BigInteger CurrentIndex { get; set; }

        [Parameter("uint256", "maxInterestRateAtFirstChange", 5)]
        public BigInteger MaxInterestRateAtFirstChange { get; set; }

        [Parameter("uint256", "minInterestRateAtFirstChange", 6)]
        public BigInteger MinInterestRateAtFirstChange { get; set; }

        [Parameter("uint256", "maxInterestRateAfterChange", 7)]
        public BigInteger MaxInterestRateAfterChange { get; set; }

        [Parameter("uint256", "minInterestRateAfterChange", 8)]
        public BigInteger MinInterestRateAfterChange { get; set; }

        public PaymentAndLateDetails()
        { }
    }
}