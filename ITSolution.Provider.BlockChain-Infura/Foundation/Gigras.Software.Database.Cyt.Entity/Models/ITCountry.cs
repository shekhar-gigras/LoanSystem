using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models;

[Table("IT_Master_Countries")]
public partial class ITCountry
{
    [Key] // Marks Id as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] // Ensures that CountryName is not null or empty
    [StringLength(255)] // Limits the length to 255 characters
    public string? CountryName { get; set; }

    [StringLength(255)] // Limits the length of IsoName to 255 characters
    public string? IsoName { get; set; }

    public int? IsoCode { get; set; } // Nullable, no need for DataAnnotation since it's int?

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

    public ICollection<ITState>? States { get; set; }
}