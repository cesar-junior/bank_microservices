using BankMicroservices.Client.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankMicroservices.Client.Model
{
    [Table("users")]
    public class User : BaseEntity
    {
        [Column("user_id")]
        [Required]
        public required string UserId { get; set; }
        [Column("full_name")]
        [Required]
        [MaxLength(150)]
        public required string FullName { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("email")]
        [MaxLength(50)]
        [Required]
        public required string Email { get; set; }
        [Column("gender")]
        [MaxLength(25)]
        public string Gender { get; set; }
        [Column("date_registered")]
        [Required]
        public DateTime DateRegistered { get; set; }
        [Column("agency")]
        [Required]
        public required int Agency { get; set; }
        [Column("account_number")]
        [Required]
        public required int AccountNumber { get; set; }
        [Column("balance")]
        [Required]
        public required float Balance { get; set; }
    }
}
