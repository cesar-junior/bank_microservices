using BankMicroservices.Email.Messages;
using BankMicroservices.Email.Repository;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BankMicroservices.Email.MessageConsumer
{
    public class RabbitMQNotificationConsumer : BackgroundService
    {
        private readonly EmailRepository _repository;
        private IConnection _connection;
        private IModel _channel;
        private const string ExchangeName = "DirectNotificationExchange";
        private const string EmailQueueName = "EmailQueueName";
        private const string RoutingKey = "Email";

        public RabbitMQNotificationConsumer(EmailRepository repository)
        {
            _repository = repository;
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(EmailQueueName, false, false, false, null);
            _channel.QueueBind(EmailQueueName, ExchangeName, RoutingKey);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (chanel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                NotificationMessage message = JsonSerializer.Deserialize<NotificationMessage>(content);
                ProcessNotification(message).GetAwaiter().GetResult();
                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume(EmailQueueName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task ProcessNotification(NotificationMessage message)
        {         
            try
            {
                await _repository.LogEmail(message);
            }
            catch (Exception)
            {
                //Log
                throw;
            }
        }
    }
}
