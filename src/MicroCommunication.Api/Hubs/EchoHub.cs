using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace MicroCommunication.Api.Hubs
{
    public class EchoHub : Hub
    {
        readonly IConfiguration configuration;

        public EchoHub(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", $"Hello from {configuration["RandomName"]}. "
                + $"I received the following message: '{message}'.");
        }
    }
}
