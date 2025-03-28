﻿using BankMicroservices.MessageBus;
using BankMicroservices.Client.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BankMicroservices.Client.RabbitMQSender
{
    public class RabbitMQLogSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _password;
        private readonly string _userName;
        private IConnection _connection;

        private const string LogQueueName = "logqueue";

        public RabbitMQLogSender()
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
                channel.QueueDeclare(queue: LogQueueName, false, false, false, arguments: null);
                byte[] body = GetMessageAsByteArray(message);
                channel.BasicPublish(
                    exchange: "", routingKey: LogQueueName, basicProperties: null, body: body);
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
