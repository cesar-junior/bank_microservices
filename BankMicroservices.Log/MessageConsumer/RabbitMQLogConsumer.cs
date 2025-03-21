using BankMicroservices.Log.Messages;
using BankMicroservices.Log.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BankMicroservices.Log.MessageConsumer
{
    public class RabbitMQLogConsumer : BackgroundService
    {
        private readonly LogRepository _repository;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQLogConsumer(LogRepository repository)
        {
            _repository = repository;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "logqueue", false, false, false, arguments: null);  
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (chanel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                LogMessage vo = JsonSerializer.Deserialize<LogMessage>(content);
                await _repository.SendNotification(vo);
                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume("logqueue", false, consumer);
            return Task.CompletedTask;
        }
    }
}
