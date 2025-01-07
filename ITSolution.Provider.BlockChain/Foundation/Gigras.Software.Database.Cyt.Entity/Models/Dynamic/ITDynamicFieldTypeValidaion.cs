using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicFieldTypeValidaion")]
    public class FieldTypeValidation
    {
        [Key]
        public int? Id { get; set; }

        public int? FieldValidationId { get; set; }
        public int? FieldTypeId { get; set; }

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

        [ForeignKey("FieldTypeId")]
        public FieldType? FieldType { get; set; }

        [ForeignKey("FieldValidationId")]
        public FieldValidation? FieldValidation { get; set; }
    }
}