using System.Threading.Tasks;

namespace MicroCommunication.Api.Abstractions
{
    public interface IRandomApi
    {

        Task<int> GetDiceAsync();
        Task<int> GetWithMaxValueAsync(int max);
    }
}
