using Common;

namespace ActiveVisitors.API.MessageService
{
    public interface IMessageProducer
    {
        public Task<List<Visitors>> SendMessage<Message, Visitors>(Message request);
    }
}
