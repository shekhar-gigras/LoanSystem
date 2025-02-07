namespace Gigras.Software.General.Model
{
    public class RequestModel
    {
        public string? SenderAddress { get; set; }
        public int? AmountInEther { get; set; }
        public string? RequestAddress { get; set; }
        public string? FunctionName { get; set; }
    }
}