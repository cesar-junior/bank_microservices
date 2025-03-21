using BankMicroservices.MessageBus;

namespace BankMicroservices.Transfer.RabbitMQSender
{
    public interface IRabbitMQMessageSender
    {
        void SendMessage(BaseMessage baseMessage);
    }
}
