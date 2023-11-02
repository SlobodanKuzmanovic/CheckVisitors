namespace ActiveVisitors.API.MessageService
{
    public interface IMessageProducer
    {
        public void SendingMessage<T>(T message);
    }
}
