using System.Threading.Tasks;
using MicroCommunication.Random.Abstractions;

namespace MicroCommunication.Random.Tests.Services
{
    public class FakeHistoryService : IHistoryService
    {
        public Task SaveValueAsync(string name, int value)
        {
            return Task.CompletedTask;
        }
    }
}
