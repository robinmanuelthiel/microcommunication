using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace MicroCommunication.Web.Pages
{
    public class IndexModel : PageModel
    {
        public readonly string RandomApiHost;
        public string Random;
        public string Error;

        public IndexModel(IConfiguration configuration)
        {
            RandomApiHost = configuration["RandomApiHost"];
        }

        public void OnGet()
        {
            try
            {
                var client = new HttpClient();
                var result = client.GetAsync(RandomApiHost).GetAwaiter().GetResult();
                var number = result.Content.ReadAsStringAsync().Result;
                Random = number;
            }
            catch (Exception ex)
            {
                Error = "Error: " + ex;
            }

        }
    }
}
