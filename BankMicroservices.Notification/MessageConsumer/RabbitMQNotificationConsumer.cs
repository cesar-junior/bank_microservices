using BankMicroservices.Notification.Messages;
using BankMicroservices.Notification.Repository;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BankMicroservices.Notification.MessageConsumer
{
    public class RabbitMQNotificationConsumer : BackgroundService
    {
        private readonly NotificationRepository _repository;
        private IConnection _connection;
        private IModel _channel;
        private const string ExchangeName = "DirectNotificationExchange";
        private const string NotificationQueueName = "NotificationQueueName";
        private const string RoutingKey = "Notification";

        public RabbitMQNotificationConsumer(NotificationRepository repository)
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

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(NotificationQueueName, false, false, false, null);
            _channel.QueueBind(NotificationQueueName, ExchangeName, RoutingKey);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (chanel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                NotificationMessage message = JsonSerializer.Deserialize<NotificationMessage>(content);
                ProcessLogs(message).GetAwaiter().GetResult();
                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume(NotificationQueueName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task ProcessLogs(NotificationMessage message)
        {         
            try
            {
                await _repository.SendNotification(message);
            }
            catch (Exception)
            {
                //Log
                throw;
            }
        }
    }
}
