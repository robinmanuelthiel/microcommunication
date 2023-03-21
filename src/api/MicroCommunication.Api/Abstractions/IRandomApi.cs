using System.Threading.Tasks;
using Refit;

namespace MicroCommunication.Api.Abstractions
{
    public interface IRandomApi
    {
        [Get("/api/dice")]
        Task<int> GetDiceAsync();

        [Get("/api/value")]
        Task<int> GetWithMaxValueAsync(int max);
    }
}
