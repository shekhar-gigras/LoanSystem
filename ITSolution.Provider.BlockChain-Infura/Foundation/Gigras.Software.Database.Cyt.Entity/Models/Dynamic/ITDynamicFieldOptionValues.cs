using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{   
    [Table("IT_DynamicFieldOptionValues")]
    public class FieldOptionValue
    {
        [Key]
        public int Id { get; set; }

        public int? OptionId { get; set; }

        [StringLength(255)]
        public string? OptionLabel { get; set; }

        [StringLength(255)]
        public string? OptionValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDelete { get; set; } = false;

        // Navigation Property for related DynamicFieldOption
        [ForeignKey("OptionId")]
        public FieldOption? Option { get; set; }
    }


}