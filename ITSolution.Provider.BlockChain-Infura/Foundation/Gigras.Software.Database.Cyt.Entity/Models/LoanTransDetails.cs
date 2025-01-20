using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    public class LoanTransDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Assuming an auto-generated primary key for the table

        [ForeignKey("LoanDetail")] // Specifies the foreign key relationship
        public int LinkId { get; set; } // Assuming an auto-generated primary key for the table

        public LoanDetails? LoanDetail { get; set; } // Assuming an auto-generated primary key for the table

        public Guid LoanId { get; set; } // Unique identifier for the loan

        [MaxLength(500)]
        public string? MetaMaskID { get; set; } // Unique identifier for the MetaMask user

        [Column(TypeName = "date")]
        public DateTime NoteDate { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength]
        public string? PropertyAddress { get; set; }

        [Column(TypeName = "decimal(15, 2)")]
        public decimal PrincipalAmount { get; set; }

        [MaxLength(100)]
        public string? LenderName { get; set; }

        [MaxLength(100)]
        public string? BorrowerName { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal InterestRate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EMIPaymentDate { get; set; }

        [Required]
        public int DayOfMonth { get; set; }

        [Column(TypeName = "date")]
        public DateTime MaturityDate { get; set; }

        [MaxLength]
        public string? PaymentLocation { get; set; }

        public int CalendarDays { get; set; }

        public int LatePaymentGracePeriod { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal LateChargePercentage { get; set; }

        [Column(TypeName = "decimal(15, 2)")]
        public decimal MonthlyPaymentAmount { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ChangeInterestRateDate { get; set; } // Nullable

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Margin { get; set; } // Nullable

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? CurrentIndex { get; set; } // Nullable

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MaxInterestRateFirstChangeDate { get; set; } // Nullable

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MinInterestRateFirstChangeDate { get; set; } // Nullable

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MaxSubsequentInterestRateAfterChangeDate { get; set; } // Nullable

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MinSubsequentInterestRateAfterChangeDate { get; set; } // Nullable

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDelete { get; set; } = false;
        public bool IsApproved { get; set; } = true;

        public bool IsRejected { get; set; } = false;
    }
}