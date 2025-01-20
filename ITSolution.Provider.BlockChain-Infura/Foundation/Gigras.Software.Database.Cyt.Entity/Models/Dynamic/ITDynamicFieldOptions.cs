using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicFieldOptions")]
    public class FieldOption
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string? OptionName { get; set; }

        public bool IsDynamic { get; set; }

        [StringLength(255)]
        public string? SourceTable { get; set; }

        [StringLength(500)]
        public string? Condition { get; set; }


        [StringLength(500)]
        public string? TextValueField { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDelete { get; set; } = false;

        [NotMapped]
        public List<string> OptionLabel { get; set; } = new List<string>();

        [NotMapped]
        public List<string> OptionValue { get; set; } = new List<string>();

        public ICollection<FieldOptionValue> OptionValues { get; set; } = new List<FieldOptionValue>();
    }
}
