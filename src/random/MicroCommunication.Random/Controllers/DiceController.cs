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
    public class DiceController : ControllerBase
    {
        readonly IHistoryService historyService;
        readonly string instanceName;
        readonly Counter diceCounter;

        public DiceController(IHistoryService historyService, IConfiguration configuration)
        {
            this.historyService = historyService;
            this.instanceName = configuration["RandomName"];

            // Custom Prometheus metrics
            diceCounter = Metrics.CreateCounter("dice_rolled", "Indicates, how often the dice has been rolled.");
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<int>> GetDice()
        {
            // Roll the dice!
            var random = new System.Random();
            var value = random.Next(1, 7);

            // Log the result
            Console.WriteLine($"The dice has been rolled: {value}");
            diceCounter.Inc();

            // Save to history
            await historyService.SaveValueAsync(instanceName, value);

            // Return the result
            return Ok(value);
        }
    }
}
