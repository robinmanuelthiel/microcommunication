using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using MicroCommunication.Api.Models;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace MicroCommunication.Api.Services
{
    public class HistoryService
    {
        readonly bool isReady;
        readonly IMongoClient client;
        readonly IMongoDatabase database;
        readonly IMongoCollection<History> collection;

        public HistoryService(string connectionString)
        {
            try
            {
                // Configure camelCase for the MongoDB driver
                var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
                ConventionRegistry.Register("camelCase", conventionPack, t => true);

                // Configure MongoDB connection
                var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

                client = new MongoClient(settings);
                database = client.GetDatabase("microcommunication");
                collection = database.GetCollection<History>("history");

                isReady = true;
            }
            catch (MongoConfigurationException)
            {
                isReady = false;
                Console.WriteLine("Warning: Invalid or no Mongo DB configuration found. If you want to save the history, please provide a valid connection string");
            }
        }

        public async Task SaveValueAsync(int value)
        {
            if (!isReady)
                return;

            try
            {
                var history = new History(value);
                await collection.InsertOneAsync(history);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: History could not be saved to Mongo DB. Please check your connection string." + ex);
            }
        }
    }
}
