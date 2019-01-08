using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return random.Next(1, 7);
        }

        // GET api/values
        [HttpGet]
        [Route("value")]
        public ActionResult<int> Get(int max)
        {
            var random = new Random();
            if (max > 0)
                return random.Next(0, max);
            else
                return random.Next();
        }
    }
}
