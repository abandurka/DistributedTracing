using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DistributedTracing.WebApp3.Controllers
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
        public async Task<int> Get(int i)
        {
            return await Task.FromResult(i);
        }
    }
}