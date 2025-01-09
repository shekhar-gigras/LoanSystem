using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    public class LoanDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Assuming an auto-generated primary key for the table

        [Required]
        public Guid LoanId { get; set; } // Unique identifier for the loan

        [MaxLength(500)]
        public string? MetaMaskID { get; set; } // Unique identifier for the MetaMask user

        [Required]
        [Column(TypeName = "date")]
        public DateTime NoteDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string? State { get; set; }

        [Required]
        [MaxLength(50)]
        public string? City { get; set; }

        [Required]
        [MaxLength]
        public string? PropertyAddress { get; set; }

        [Required]
        [Column(TypeName = "decimal(15, 2)")]
        public decimal PrincipalAmount { get; set; }

        [Required]
        [MaxLength(100)]
        public string? LenderName { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal InterestRate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EMIPaymentDate { get; set; }

        [Required]
        public int DayOfMonth { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime MaturityDate { get; set; }

        [Required]
        [MaxLength]
        public string? PaymentLocation { get; set; }

        [Required]
        public int CalendarDays { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal LateChargePercentage { get; set; }

        [Required]
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
    }
}