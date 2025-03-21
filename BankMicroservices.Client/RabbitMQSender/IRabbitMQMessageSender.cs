using BankMicroservices.MessageBus;

namespace BankMicroservices.Client.RabbitMQSender
{
    public interface IRabbitMQMessageSender
    {
        void SendMessage(BaseMessage baseMessage);
    }
}
