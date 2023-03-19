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
    public class RandomController : ControllerBase
    {
        readonly IHistoryService historyService;
        readonly string instanceName;
        readonly Counter diceCounter;
        readonly Counter randomCounter;

        public RandomController(IHistoryService historyService, IConfiguration configuration)
        {
            this.historyService = historyService;
            this.instanceName = configuration["RandomName"];

            // Custom Prometheus metrics
            diceCounter = Metrics.CreateCounter("dice_rolled", "Indicates, how often the dice has been rolled.");
            randomCounter = Metrics.CreateCounter("random_number_generated", "Indicates, how often a random number has been generated.");
        }

        // GET api/values
        [HttpGet]
        [Route("dice")]
        public async Task<ActionResult<int>> GetDice()
        {
            // Roll the dice!
            var random = new Random();
            var value = random.Next(1, 7);

            // Log the result
            Console.WriteLine($"The dice has been rolled: {value}");
            diceCounter.Inc();

            // Save to history
            await historyService.SaveValueAsync(instanceName, value);

            // Return the result
            return Ok(value);
        }

        // GET api/values
        [HttpGet]
        [Route("value")]
        public async Task<ActionResult<int>> Get(int max)
        {
            var random = new Random();
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
