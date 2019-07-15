using System;
using Microsoft.AspNetCore.Mvc;

namespace MicroCommunication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Route("dice")]
        public ActionResult<int> GetDice()
        {
            // Roll the dice!
            var random = new Random();
            var value = random.Next(1, 7);

            // Log the result
            Console.WriteLine($"The dice has been rolled: {value}");

            // Return the result
            return Ok(value);
        }

        // GET api/values
        [HttpGet]
        [Route("value")]
        public ActionResult<int> Get(int max)
        {
            var random = new Random();
            int value;
            if (max > 0)
                value = random.Next(0, max);
            else
                value = random.Next();

            // Log the result
            Console.WriteLine($"A random number has been generated: {value}");

            // Return the result
            return Ok(value);
        }
    }
}
