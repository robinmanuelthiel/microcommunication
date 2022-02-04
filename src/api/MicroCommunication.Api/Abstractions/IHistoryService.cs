using System.Threading.Tasks;

namespace MicroCommunication.Api.Abstractions
{
    public interface IHistoryService
    {
        Task SaveValueAsync(string name, int value);
    }
}
