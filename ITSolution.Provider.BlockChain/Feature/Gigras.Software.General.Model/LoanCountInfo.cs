using Gigras.Software.Database.Cyt.Entity.Models;

namespace Gigras.Software.General.Model
{
    public class LoanCountInfo
    {
        public List<LoanDetails> Records { get; set; }
        public int CountDeleted { get; set; }
        public int CountApproved { get; set; }
        public int CountRejected { get; set; }
        public int CountPending { get; set; }
        public decimal DeletedAmount { get; set; }
        public decimal ApprovedAmount { get; set; }
        public decimal RejectedAmount { get; set; }
        public decimal PendingAmount { get; set; }
    }
}