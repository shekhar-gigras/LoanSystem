using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Gigras.Software.BlockChain.Service.Model
{
    [FunctionOutput]
    public class Loan
    {
        [Parameter("tuple", "commonAndInterestDetails", 1)]
        public CommonAndInterestDetails CommonAndInterestDetails { get; set; }

        [Parameter("tuple", "paymentAndLateDetails", 2)]
        public PaymentAndLateDetails PaymentAndLateDetails { get; set; }

        [Parameter("tuple", "adjustableInterestRateDetails", 3)]
        public AdjustableInterestRateDetails AdjustableInterestRateDetails { get; set; }

        // Parameterless constructor is required by Nethereum
        public Loan()
        { }
    }
}