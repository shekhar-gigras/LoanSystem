using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicFormsSection")]
    public class FormsSection
    {
        [Key]
        public int Id { get; set; }

        public int? FormId { get; set; }

        [Required]
        [StringLength(255)]
        public string SectionName { get; set; } = string.Empty;

        public string? SectionDescription { get; set; }
        public int? SortOrder { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsDelete { get; set; }

        [ForeignKey("FormId")]
        public Form? Form { get; set; }

        // Navigation property (Optional)
        public List<FormField>? FormFields { get; set; } = new List<FormField>();
    }
}