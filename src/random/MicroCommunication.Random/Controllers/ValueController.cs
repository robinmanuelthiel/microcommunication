using System;
using System.Threading.Tasks;
using MicroCommunication.Random.Abstractions;
using MicroCommunication.Random.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Prometheus;

namespace MicroCommunication.Random.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValueController : ControllerBase
    {
        readonly IHistoryService historyService;
        readonly string instanceName;
        readonly Counter randomCounter;

        public ValueController(IHistoryService historyService, IConfiguration configuration)
        {
            this.historyService = historyService;
            this.instanceName = configuration["RandomName"];

            // Custom Prometheus metrics
            randomCounter = Metrics.CreateCounter("random_number_generated", "Indicates, how often a random number has been generated.");
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<int>> Get(int max)
        {
            var random = new System.Random();
            int value;
            if (max > 0)
                value = random.Next(0, max);
            else
                value = random.Next();

            // Log the result
            Console.WriteLine($"A random number has been generated: {value}");
            randomCounter.Inc();

            // Save to history
            await historyService.SaveValueAsync(instanceName, value);

            // Return the result
            return Ok(value);
        }
    }
}
