using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models;

[Table("IT_Admin")]
public class ITAdmin
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Guid? UserId { get; set; }

    [StringLength(255)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? UserName { get; set; }

    public string? Role { get; set; }

    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [StringLength(255)]
    public string? Password { get; set; }

    [StringLength(255)]
    [Phone]
    public string? Phone { get; set; }

    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public DateTime? LastLogin { get; set; } = DateTime.Now;

    public bool IsActive { get; set; }
    public bool IsDelete { get; set; }
    public bool IsBlock { get; set; }
    public bool IsAddLoan { get; set; }
    public bool IsEditLoan { get; set; }
    public bool IsDeleteLoan { get; set; }
    public bool IsVisibleLoanSale { get; set; }

    public bool IsConfirmLink { get; set; }

    [StringLength(500)]
    public string? Token { get; set; }

    public DateTime? TokenExpiry { get; set; }
}