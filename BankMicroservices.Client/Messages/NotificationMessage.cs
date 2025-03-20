using BankMicroservices.MessageBus;

namespace BankMicroservices.Client.Messages
{
    public class NotificationMessage : BaseMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool SendEmail { get; set; }
    }
}
