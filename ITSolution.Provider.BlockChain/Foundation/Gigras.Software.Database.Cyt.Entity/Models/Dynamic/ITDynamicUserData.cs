using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gigras.Software.Database.Cyt.Entity.Models
{
    [Table("IT_DynamicUserData")]
    public class DynamicUserData
    {
        [Key]
        public int Id { get; set; }

        public int? FormId { get; set; }

        public string? MetaMaskID { get; set; }

        public string? UserData { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDelete { get; set; } = false;

        [ForeignKey("FormId")]
        public Form? Form { get; set; }

        public int? OldUserId { get; set; }
    }
}