using BankMicroservices.Notification.Model.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankMicroservices.Notification.Model
{
    [Table("notifications")]
    public class NotificationModel : BaseEntity
    {
        [Column("userId")]
        public string UserId { get; set; }

        [Column("title")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Column("message")]
        [MaxLength(500)]
        public string Message { get; set; }

        [Column("sent_date")]
        public DateTime SentDate { get; set; }

        [Column("read")]
        public bool Read { get; set; }
    }
}
