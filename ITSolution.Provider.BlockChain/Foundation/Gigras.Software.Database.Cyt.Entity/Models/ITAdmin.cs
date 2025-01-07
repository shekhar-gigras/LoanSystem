using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models;

[Table("IT_Admin")]
public class ITAdmin
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

    [StringLength(500)]
    public string? Avatar { get; set; }

    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public DateTime? LastLogin { get; set; } = DateTime.Now;

    public bool? IsActive { get; set; }

    [StringLength(500)]
    public string? Token { get; set; }

    public DateTime? TokenExpiry { get; set; }

    [NotMapped]
    public IFormFile? AvatarFile { get; set; } // For handling file uploads
}