using System;
using System.Threading.Tasks;
using MicroCommunication.Api.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Prometheus;

namespace MicroCommunication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomController : ControllerBase
    {
        readonly IRandomApi _randomApi;

        public RandomController(IRandomApi randomApi)
        {
            _randomApi = randomApi;
        }

        // GET api/values
        [HttpGet]
        [Route("dice")]
        public async Task<ActionResult<int>> GetDice()
        {
            // Roll the dice!
            var value = await _randomApi.GetDiceAsync();

            // Return the result
            return Ok(value);
        }

        // GET api/values
        [HttpGet]
        [Route("value")]
        public async Task<ActionResult<int>> Get(int max)
        {
            // Get max value
            var value = await _randomApi.GetWithMaxValueAsync(max);

            // Return the result
            return Ok(value);
        }
    }
}
