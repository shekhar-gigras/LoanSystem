using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicFieldTypes")]
    public class FieldType
    {
        [Key]
        public int Id { get; set; }

        public int? FieldTypeOptionId { get; set; }

        [Required]
        [StringLength(50)]
        public string? CtrlType { get; set; }

        [Required]
        [StringLength(50)]
        public string? FieldName { get; set; }

        [StringLength(255)]
        public string? FieldDescription { get; set; }

        [Required]
        public bool HasOptions { get; set; } = false;

        [Required]
        public bool RequiresScript { get; set; } = false;

        public string? DefaultValue { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public int? MaxLength { get; set; }
        public decimal? Steps { get; set; }

        public int? BlockChainFieldId { get; set; }

        public int? ComparerFieldId { get; set; }

        public int? RangeStart { get; set; }

        public int? RangeEnd { get; set; }
        [MaxLength]
        public string? JavaScript { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; } = 1;

        public int UpdatedBy { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;

        [ForeignKey("FieldTypeOptionId")]
        public FieldOption? FieldOption { get; set; }

        public List<FieldTypeValidation>? FieldTypeValidations { get; set; }
    }
}