using System.Threading.Tasks;
using MicroCommunication.Api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace MicroCommunication.Api.Hubs
{
    public class ChatHub : Hub
    {
        readonly string serverName;

        public ChatHub(IConfiguration configuration)
        {
            serverName = configuration["RandomName"];
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Name", serverName);
        }

        public async Task SendMessage(string text, string sender)
        {
            var chatMessage = new ChatMessage(text, sender, serverName);
            await Clients.All.SendAsync("ReceiveMessage", chatMessage);
        }
    }
}
