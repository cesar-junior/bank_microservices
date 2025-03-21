using BankMicroservices.Log.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankMicroservices.Log.Model
{
    [Table("logs")]
    public class LogModel : BaseEntity
    {
        [Column("type")]
        [MaxLength(50)]
        [Required]
        public required string Type { get; set; }

        [Column("message")]
        [Required]
        public required string Message { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }
    }
}
