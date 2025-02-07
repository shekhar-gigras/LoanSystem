using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Gigras.Software.BlockChain.Service.Model
{
    [FunctionOutput]
    public class OtherDetails : AdjustableInterestRateDetails
    {
        public string? LoanId { get; set; }
    }

    [FunctionOutput]
    public class AdjustableInterestRateDetails
    {
        [Parameter("string", "noteDate", 1)]
        public string NoteDate { get; set; }

        [Parameter("string", "city", 2)]
        public string City { get; set; }

        [Parameter("string", "state", 3)]
        public string State { get; set; }

        [Parameter("string", "propertyAddress", 4)]
        public string PropertyAddress { get; set; }

        [Parameter("string", "paymentLocation", 5)]
        public string PaymentLocation { get; set; }

        public AdjustableInterestRateDetails()
        { }
    }
}