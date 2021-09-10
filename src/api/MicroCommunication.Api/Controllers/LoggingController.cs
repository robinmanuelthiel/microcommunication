using MicroCommunication.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MicroCommunication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        readonly ILogger<LoggingController> logger;

        public LoggingController(ILogger<LoggingController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("generate")]
        public ActionResult Generate()
        {
            logger.LogInformation("Demo Log Information 1");
            logger.LogWarning("Demo Log Warning 1");
            logger.LogError("Demo Log Error 1");

            return Ok();
        }
    }
}
