using System.Net.Http;
using System.Threading.Tasks;
using MicroCommunication.Random.Abstractions;
using MicroCommunication.Random.Models;
using Microsoft.Azure.Cosmos;

namespace MicroCommunication.Random.Services
{
    public class CosmosDbHistoryService : IHistoryService
    {
        readonly CosmosClient client;

        public CosmosDbHistoryService(string connectionString)
        {
            client = new CosmosClient(connectionString, new CosmosClientOptions()
            {
                HttpClientFactory = () =>
                {
                    HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
                    };
                    return new HttpClient(httpMessageHandler);
                },
                ConnectionMode = ConnectionMode.Gateway,
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });
        }

        public async Task SaveValueAsync(string name, int value)
        {
            // Create Database and Container if not existent
            await client.CreateDatabaseIfNotExistsAsync("microcommunication");
            var database = client.GetDatabase("microcommunication");
            await database.CreateContainerIfNotExistsAsync("history", "/name");
            var container = database.GetContainer("history");

            // Add item to container
            await container.UpsertItemAsync(new History(name, value));
        }
    }
}
