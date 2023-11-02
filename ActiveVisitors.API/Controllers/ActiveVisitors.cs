using ActiveVisitors.API.MessageService;
using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ActiveVisitors.API.Controllers
{
    [ApiController]
    [Route("api/active-visitors")]
    public class ActiveVisitorsController : ControllerBase
    {

        private readonly IMessageProducer _messageProducer;

        public ActiveVisitorsController(IMessageProducer messageProducer)
        {
                _messageProducer = messageProducer;
        }

        [HttpGet(Name = "GetActiveVisitors")]
        public string GetActiveVisitors(string date, string? camerraIds = "")
        {
            Message message = new Message()
            {
                date = date,
                cameras = camerraIds
            };

            _messageProducer.SendingMessage<Message>(message);

            return "Slobodan";
        }
    }
}
