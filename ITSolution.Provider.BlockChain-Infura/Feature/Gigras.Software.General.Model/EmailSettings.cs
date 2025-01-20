namespace Gigras.Software.General.Model
{
    public class EmailSettings
    {
        public string? FromEmailID { get; set; }
        public string? FromDisplayName { get; set; }
        public string? ToEmailID { get; set; }
        public string? UserName { get; set; }
        public string? IsLive { get; set; }
        public int Port { get; set; }
        public string? Server { get; set; }
        public bool SslRequest { get; set; }
        public string? Password { get; set; }
    }
}