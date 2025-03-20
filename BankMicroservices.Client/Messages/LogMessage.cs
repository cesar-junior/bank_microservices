using BankMicroservices.MessageBus;

namespace BankMicroservices.Client.Messages
{
    public class LogMessage : BaseMessage
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
