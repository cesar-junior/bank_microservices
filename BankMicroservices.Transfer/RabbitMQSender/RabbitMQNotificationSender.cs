using BankMicroservices.MessageBus;
using BankMicroservices.Transfer.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BankMicroservices.Transfer.RabbitMQSender
{
    public class RabbitMQNotificationSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _password;
        private readonly string _userName;
        private IConnection _connection;

        private const string ExchangeName = "DirectNotificationExchange";
        private const string NotificationQueueName = "NotificationQueueName";
        private const string NotificationRoutingKey = "Notification";
        private const string EmailQueueName = "EmailQueueName";
        private const string EmailRoutingKey = "Email";

        public RabbitMQNotificationSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";
        }

        public void SendMessage(BaseMessage message)
        {
            if(ConnectionExists())
            {
                using var channel = _connection.CreateModel();

                channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: false);
                channel.QueueDeclare(NotificationQueueName, false, false, false, null);
                channel.QueueDeclare(EmailQueueName, false, false, false, null);

                channel.QueueBind(NotificationQueueName, ExchangeName, NotificationRoutingKey);
                channel.QueueBind(EmailQueueName, ExchangeName, EmailRoutingKey);

                byte[] body = GetMessageAsByteArray(message);
                channel.BasicPublish(
                    exchange: ExchangeName, NotificationRoutingKey, basicProperties: null, body: body);
                channel.BasicPublish(
                    exchange: ExchangeName, EmailRoutingKey, basicProperties: null, body: body);
            }
        }

        private byte[] GetMessageAsByteArray(BaseMessage message)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var json = JsonSerializer.Serialize<LogMessage>((LogMessage)message, options);
            var body = Encoding.UTF8.GetBytes(json);
            return body;
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception)
            {
                //Log exception
                throw;
            }
        }

        private bool ConnectionExists()
        {   
            if(_connection != null) return true;
            CreateConnection();
            return _connection != null;
        }
    }
}
