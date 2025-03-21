using BankMicroservices.Transfer.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BankMicroservices.Transfer.Model
{
    [Table("transfer")]
    public class TransferModel : BaseEntity
    {
        [Required]
        [Column("sender_user_id")]
        public required string SenderUserId { get; set; }

        [Required]
        [Column("receiver_user_id")]
        public required string ReceiverUserId { get; set; }

        [AllowNull]
        [MaxLength(50)]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("status")]
        public int Status { get; set; }

        [Required]
        [Column("sent_date")]
        public DateTime SentDate { get; set; }

        [AllowNull]
        [Column("received_date")]
        public DateTime? ReceivedDate { get; set; }

        [Required]
        [Column("amount")]
        public float Amount { get; set; }
    }
}
