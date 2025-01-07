using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicBlockChainField")]
    public class DynamicBlockChainField
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string? BlockChainField { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDelete { get; set; } = false;
    }
}