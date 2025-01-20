using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models;


[Table("IT_Users")]
public class ITUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(255)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? UserName { get; set; }

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

    [StringLength(255)]
    public string? Avatar { get; set; }

    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public DateTime? LastLogin { get; set; } = DateTime.Now;

    public bool? Active { get; set; } = false;
}