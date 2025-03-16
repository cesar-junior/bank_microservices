using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace BankMicroservices.Email.Data.ValueObjects
{
    public class EmailVO
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Log { get; set; }
        public DateTime SentDate { get; set; }
    }
}
