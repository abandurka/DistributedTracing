using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DistributedTracing.WebApp2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TracingController : ControllerBase
    {
        private readonly ILogger<TracingController> _logger;
        private readonly IEventSender _eventSender;

        public TracingController(ILogger<TracingController> logger, IEventSender eventSender)
        {
            _logger = logger;
            _eventSender = eventSender;
        }

        [HttpGet]
        public async Task<string> Get(string i)
        {
            await _eventSender.SendAsync(i);
            
            return i;
        }
    }
}