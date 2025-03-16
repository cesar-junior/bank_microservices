using System.Threading.Tasks;

namespace BankMicroservices.MessageBus
{
    public interface IMessageBus
    {
        Task PublicMessage(BaseMessage message, string queueName);
    }
}
