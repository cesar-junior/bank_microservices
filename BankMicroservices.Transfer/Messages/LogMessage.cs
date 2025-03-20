using BankMicroservices.MessageBus;

namespace BankMicroservices.Transfer.Messages
{
    public class LogMessage : BaseMessage
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
