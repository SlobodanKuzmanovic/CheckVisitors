// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Hello, World!");

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "root",
    Password = "password",
    VirtualHost = "/"
};

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.QueueDeclare("sightings", durable: false, exclusive: false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine(message);
};

channel.BasicConsume("sightings", true, consumer);

Console.ReadKey();