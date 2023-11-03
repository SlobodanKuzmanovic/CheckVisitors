using Amazon.Runtime.Internal;
using Common;
using DBAccessLayer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Server is running!");

        var factory = Helper.CreateFactory();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(QueueName.REQUEST_QUEUE.ToString(), exclusive: false);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var jsonSerialized = Encoding.UTF8.GetString(ea.Body.ToArray());

            var obj = JsonSerializer.Deserialize<Message>(jsonSerialized);

            SightingsDBAccess db = new SightingsDBAccess();

            var visitorsByHour = db.ReadData(Convert.ToDateTime(obj.date), obj.cameras);


            var replyMess = JsonSerializer.Serialize(visitorsByHour);
            var body = Encoding.UTF8.GetBytes(replyMess);

            var properties = channel.CreateBasicProperties();
            properties.CorrelationId = ea.BasicProperties.CorrelationId;

            channel.BasicPublish("", ea.BasicProperties.ReplyTo, properties, body);
        };


        channel.BasicConsume(QueueName.REQUEST_QUEUE.ToString(), autoAck: true, consumer: consumer);

        Console.ReadLine();
    }
}