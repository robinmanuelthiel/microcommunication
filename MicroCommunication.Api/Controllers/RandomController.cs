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
        public ActionResult<int> Get()
        {
            // Roll the dice!
            var random = new Random();
            return random.Next(1, 7);
        }
    }
}
