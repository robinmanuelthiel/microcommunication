using System;
using System.Threading.Tasks;
using MicroCommunication.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MicroCommunication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomController : ControllerBase
    {
        readonly HistoryService historyService;

        public RandomController(HistoryService historyService)
        {
            this.historyService = historyService;
        }

        // GET api/values
        [HttpGet]
        [Route("dice")]
        public async Task<ActionResult<int>> GetDice()
        {
            // Roll the dice!
            var random = new Random();
            var value = random.Next(1, 7);

            // Save to history
            await historyService.SaveValueAsync(value);

            // Log the result
            Console.WriteLine($"The dice has been rolled: {value}");

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

            // Save to history
            await historyService.SaveValueAsync(value);

            // Return the result
            return Ok(value);
        }
    }
}
