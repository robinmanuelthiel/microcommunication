using System;
using System.Threading.Tasks;
using MicroCommunication.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MicroCommunication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameController : ControllerBase
    {
        readonly IConfiguration configuration;

        public NameController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // GET api/values
        [HttpGet]
        [Route("dice")]
        public async Task<ActionResult<string>> Get()
        {
            return Ok(configuration["RandomName"]);
        }
    }
}
