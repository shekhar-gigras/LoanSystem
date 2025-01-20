using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    public class BorrowerLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary key with auto-increment

        [Required]
        public Guid LoanId { get; set; } // Unique identifier for the loan

        [MaxLength(500)]
        public string? MetaMaskID { get; set; } // Unique identifier for the MetaMask user

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? InitialMonthlyPayment { get; set; } // Initial monthly payment

        public DateTime? FirstPaymentDate { get; set; } // First payment date

        public DateTime? MaturityDate { get; set; } // Loan maturity date

        [MaxLength(255)]
        public string? PaymentAddress { get; set; } // Payment address

        public int? GracePeriod { get; set; } // Grace period in days

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? LateChargePercentage { get; set; } // Late charge percentage

        [MaxLength(255)]
        public string? IndexType { get; set; } // Index (e.g., LIBOR, SOFR)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Margin { get; set; } // Margin for the loan

        public bool? AllowsPrepayment { get; set; } // Prepayment allowed or not

        public int? DefaultNoticePeriod { get; set; } // Default notice period in days

        public DateTime? NoteDate { get; set; } // Note date

        [MaxLength(100)]
        public string? CountryName { get; set; } // Property country

        [MaxLength(100)]
        public string? CityName { get; set; } // Property city

        [MaxLength(50)]
        public string? StateName { get; set; } // Property state

        [MaxLength(255)]
        public string? PropertyAddress { get; set; } // Full property address

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PrincipalAmount { get; set; } // Principal amount of the loan

        [MaxLength(255)]
        public string? LenderName { get; set; } // Name of the lender

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? InitialInterestRate { get; set; } // Initial interest rate

        public DateTime? ChangeDate { get; set; } // Interest rate change date

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MaxRateOnFirstChange { get; set; } // Maximum rate on first change

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MinRateOnFirstChange { get; set; } // Minimum rate on first change

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MaxOverallRate { get; set; } // Maximum overall rate

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? MinOverallRate { get; set; } // Minimum overall rate

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDelete { get; set; } = false;
    }
}