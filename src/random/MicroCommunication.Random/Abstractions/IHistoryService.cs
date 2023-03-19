using System.Threading.Tasks;

namespace MicroCommunication.Random.Abstractions
{
    public interface IHistoryService
    {
        Task SaveValueAsync(string name, int value);
    }
}
