using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace BankMicroservices.Notification.Data.ValueObjects
{
    public class NotificationVO
    {
        public long Id { get; set; }
        public string UserId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime SentDate { get; set; }

        public bool Read { get; set; }
    }
}
