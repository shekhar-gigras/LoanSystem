using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicForms")]
    public class Form
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? FormName { get; set; }

        public string? FormDescription { get; set; }
        public string? EntityName { get; set; }
        public string? NavigationGroup { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 1;
        public int UpdatedBy { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;

        [ForeignKey("CountryId")]
        public ITCountry? Country { get; set; }

        [ForeignKey("StateId")]
        public ITState? State { get; set; }

        [ForeignKey("CityId")]
        public ITCity? City { get; set; }

        public List<FormsSection>? FormsSections { get; set; } = new List<FormsSection>();

        [NotMapped]
        public Dictionary<string, object>? FieldValues { get; set; }
    }
}