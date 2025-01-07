using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicValidation")]
    public class DynamicValidation
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string? ValidationType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsDelete { get; set; }
    }
}