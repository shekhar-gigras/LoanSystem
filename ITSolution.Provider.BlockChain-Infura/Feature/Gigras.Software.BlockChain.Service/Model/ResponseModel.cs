namespace Gigras.Software.BlockChain.Service.Model
{
    public class ResponseModel
    {
        public bool Status { get; set; } = false;
        public string? Message { get; set; } = string.Empty;
        public string? TransactionCode { get; set; } = string.Empty;
        public object? Result { get; set; }
        public object? Final { get; set; }
    }
}