namespace Gigras.Software.General.Model
{
    public class TransactionRequestModel
    {
        public string? SenderAddress { get; set; }
        public string? FunctionName { get; set; }
        public decimal AmountInEther { get; set; }
    }
}