using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ActiveVisitors.API.Controllers
{
    [ApiController]
    [Route("api/active-visitors")]
    public class ActiveVisitorsController : ControllerBase
    {
        [HttpGet(Name = "Index")]
        public string Index(DateTime date, string camerraIds)
        {
            return "Slobodan";
        }
    }
}
