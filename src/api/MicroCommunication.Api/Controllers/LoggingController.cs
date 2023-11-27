using MicroCommunication.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System;

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

        [HttpGet]
        [Route("long-running-trace")]
        public ActionResult<string> LongRunningWithTrace()
        {
            using (var fistActivity = Observability.DefaultActivities.StartActivity("First section"))
            {
                fistActivity.AddTag("date", DateTime.UtcNow.ToString());
                fistActivity.AddTag("section", "first");
                Thread.Sleep(100);
                fistActivity.Stop();
            };

            using (var secondActivity = Observability.DefaultActivities.StartActivity("Second section"))
            {
                secondActivity.AddTag("date", DateTime.UtcNow.ToString());
                secondActivity.AddTag("section", "second");
                Thread.Sleep(200);
                secondActivity.Stop();
            };

            using (var thirdActivity = Observability.DefaultActivities.StartActivity("Second section"))
            {
                thirdActivity.AddTag("date", DateTime.UtcNow.ToString());
                thirdActivity.AddTag("section", "third");
                Thread.Sleep(300);
                thirdActivity.Stop();
            };

            return Ok("Hello World!");
        }
    }
}
