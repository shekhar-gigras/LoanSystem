using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models;

[Table("IT_Master_Cities")]
public partial class ITCity
{
    [Key] // Marks Id as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] // Ensures that CityName is not null
    [StringLength(255)] // Limits the length to 255 characters
    public string? CityName { get; set; }

    [Required] // Ensures that CountryId is not null
    [ForeignKey("Country")] // Specifies the foreign key relationship
    public int CountryId { get; set; }

    public ITCountry? Country { get; set; } // Navigation property

    [Required] // Ensures that CountryId is not null
    [ForeignKey("State")] // Specifies the foreign key relationship
    public int StateId { get; set; }

    public ITState? State { get; set; } // Navigation property

    [StringLength(255)] // Limits the length of CreatedBy to 255 characters
    public string? CreatedBy { get; set; }

    [Required] // Ensures CreatedDate is not null
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [StringLength(255)] // Limits the length of ModifiedBy to 255 characters
    public string? ModifiedBy { get; set; }

    [Required] // Ensures ModifiedDate is not null
    public DateTime ModifiedDate { get; set; } = DateTime.Now;

    [Required] // Ensures IsActive is not null
    public bool IsActive { get; set; } = true;

    [Required] // Ensures IsDelete is not null
    public bool IsDelete { get; set; } = false;
}