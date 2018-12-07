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
        readonly string randomApiHost;

        public IndexModel(IConfiguration configuration)
        {
            randomApiHost = configuration["RandomApiHost"];
        }

        public void OnGet()
        {           
            try
            {
                var client = new HttpClient();
                var result = client.GetAsync(randomApiHost).GetAwaiter().GetResult();
                var number = result.Content.ReadAsStringAsync().Result;
                ViewData["Random"] = number;
            }
            catch (Exception ex)
            {
                ViewData["Random"] = "Error: " + ex;
            }

        }
    }
}
