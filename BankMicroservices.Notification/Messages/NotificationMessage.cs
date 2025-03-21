namespace BankMicroservices.Notification.Messages
{
    public class NotificationMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool SendEmail { get; set; }
    }
}
