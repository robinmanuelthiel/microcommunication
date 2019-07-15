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

        private readonly string randomApiKey;
        private readonly string ocpApimSubscriptionKey;

        public IndexModel(IConfiguration configuration)
        {
            RandomApiHost = configuration["RandomApiHost"];
            randomApiKey = configuration["RandomApiKey"];
            ocpApimSubscriptionKey = configuration["OcpApimSubscriptionKey"];
        }

        public void OnGet()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ocpApimSubscriptionKey);
                client.DefaultRequestHeaders.Add("api-key", randomApiKey);

                var result = client.GetAsync(RandomApiHost + "/dice").GetAwaiter().GetResult();
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
