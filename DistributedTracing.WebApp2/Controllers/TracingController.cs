using System.Net.Http;
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

        public TracingController(ILogger<TracingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get(string i)
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync("http://localhost:5002/tracing?i=" + i);
            
            return response;
        }
    }
}