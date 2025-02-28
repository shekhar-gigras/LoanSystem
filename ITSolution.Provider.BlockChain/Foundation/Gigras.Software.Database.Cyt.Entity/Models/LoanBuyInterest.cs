using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("LoanBuyInterest")]
    public class LoanBuyInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid? LoanId { get; set; }
        public int LoanDetailId { get; set; }

        [ForeignKey(nameof(LoanDetailId))]
        public LoanDetails? LoanDetails { get; set; }

        public Guid SellerId { get; set; }
        public Guid BuyerId { get; set; }

        public bool IsInterested { get; set; } = false;
        public bool IsDenied { get; set; } = false;
        public bool IsSellerApproved { get; set; } = false;
        public bool IsAdminApproved { get; set; } = false;
        public bool IsClosedSellerLoan { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDelete { get; set; } = false;

        [NotMapped]
        public string? LendderName { get; set; }
        [NotMapped]
        public string? LendderEmail { get; set; }
        [NotMapped]
        public string? LendderPhone { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DealAmount { get; set; } = 0;
        public string? Comments { get; set; } = "";

    }
}