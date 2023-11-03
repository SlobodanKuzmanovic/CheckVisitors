using Common;

namespace ActiveVisitors.API.MessageService
{
    public interface IMessageProducerWithReply
    {
        public Task<List<Visitors>> SendMessage<Message, Visitors>(Message request);
    }
}
