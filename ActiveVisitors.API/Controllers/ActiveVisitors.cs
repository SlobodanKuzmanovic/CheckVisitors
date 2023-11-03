using ActiveVisitors.API.MessageService;
using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ActiveVisitors.API.Controllers
{
    [ApiController]
    [Route("api/active-visitors")]
    public class ActiveVisitorsController : ControllerBase
    {

        private readonly IMessageProducer _messageProducerWithReply;

        public ActiveVisitorsController(
            IMessageProducer messageProducerWithReply)
        {
            _messageProducerWithReply = messageProducerWithReply;
        }

        [HttpGet(Name = "GetActiveVisitors")]
        public async Task<IActionResult> GetActiveVisitors(string date, string? camerraIds = "")
        {
            Message message = new Message()
            {
                date = date,
                cameras = camerraIds
            };

            var response = await _messageProducerWithReply.SendMessage<Message, Visitors>(message);

            return Ok(response);
        }
    }
}
