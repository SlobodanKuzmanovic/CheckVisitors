﻿using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ActiveVisitors.API.MessageService
{
    public class MessageProducer : IMessageProducer

    {
        public void SendingMessage<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "root",
                Password = "password",
                VirtualHost = "/"
            };

            var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare("sightings", durable: true, exclusive: true);

            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            channel.BasicPublish("", "sightings", body: body);
        }
    }
}
