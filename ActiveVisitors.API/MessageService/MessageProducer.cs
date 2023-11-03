using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ActiveVisitors.API.MessageService
{
    public class MessageProducer : IMessageProducer
    {

        public async Task<List<Visitors>> SendMessage<Message, Visitors>(Message request)
        {
            var factory = Helper.CreateFactory();
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(QueueName.REQUEST_QUEUE.ToString(), durable: false, exclusive: false);

            var correlationId = Guid.NewGuid().ToString();

            var replyQueueName = channel.QueueDeclare(QueueName.REPLY_QUEUE.ToString(), durable: false, exclusive: false).QueueName;

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(replyQueueName, true, consumer);

            var requestJson = JsonSerializer.Serialize(request);
            var requestBytes = Encoding.UTF8.GetBytes(requestJson);

            var properties = channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = replyQueueName;

            channel.BasicPublish("", QueueName.REQUEST_QUEUE.ToString(), body: requestBytes, basicProperties: properties);

            var responseCompletionSource = new TaskCompletionSource<List<Visitors>>();

            consumer.Received += (sender, args) =>
            {
                if (args.BasicProperties.CorrelationId == correlationId)
                {
                    var responseJson = Encoding.UTF8.GetString(args.Body.ToArray());
                    var response = JsonSerializer.Deserialize<List<Visitors>>(responseJson);
                    responseCompletionSource.SetResult(response);
                }
            };

            return await responseCompletionSource.Task;
        }
    }
}
