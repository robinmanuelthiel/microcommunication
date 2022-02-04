using System.Threading.Tasks;
using MicroCommunication.Api.Abstractions;

namespace MicroCommunication.Api.Tests.Services
{
    public class FakeHistoryService : IHistoryService
    {
        public Task SaveValueAsync(string name, int value)
        {
            return Task.CompletedTask;
        }
    }
}
