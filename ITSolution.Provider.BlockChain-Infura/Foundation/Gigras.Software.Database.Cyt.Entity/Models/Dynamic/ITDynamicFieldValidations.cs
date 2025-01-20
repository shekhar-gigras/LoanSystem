using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicFieldValidations")]
    public class FieldValidation
    {
        [Key]
        public int Id { get; set; }

        public string? ValidationName { get; set; }

        [Required]
        [StringLength(50)]
        public string? ValidationType { get; set; }

        [StringLength(500)]
        public string? ValidationValue { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; } = 1;

        public int UpdatedBy { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;

        public List<FieldTypeValidation>? FieldTypeValidaions { get; set; }
    }
}