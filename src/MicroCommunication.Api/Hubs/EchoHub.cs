using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MicroCommunication.Api.Hubs
{
    public class EchoHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", "Message received: " + message);
        }
    }
}
