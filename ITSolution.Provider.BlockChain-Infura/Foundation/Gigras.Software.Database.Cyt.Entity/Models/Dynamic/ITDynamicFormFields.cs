using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicFormFields")]
    public class FormField
    {
        [Key]
        public int Id { get; set; }

        public int SectionId { get; set; }

        public int? FieldTypeId { get; set; }

        public int? FieldOrder { get; set; }

        [MaxLength]
        public string? CssClass { get; set; }

        [MaxLength]
        public string? JavaScript { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; } = 1;

        public int UpdatedBy { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;

        // Navigation properties
        [ForeignKey("SectionId")]
        public FormsSection? FormSection { get; set; }

        [ForeignKey("FieldTypeId")]
        public FieldType? FieldType { get; set; }

        [NotMapped]
        public string? FieldValue { get; set; }
    }
}