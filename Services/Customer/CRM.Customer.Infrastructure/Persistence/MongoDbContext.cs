using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CRM.Customer.Domain.Entities;
using CRM.Customer.Infrastructure.Persistence.Configurations;

namespace CRM.Customer.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private static IMongoClient? _client;
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var mongoSettings = settings.Value;
            if (_client == null)
            {
                var clientSettings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
                clientSettings.ConnectTimeout = TimeSpan.FromSeconds(10);
                clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
                clientSettings.MaxConnectionPoolSize = 100;
                _client = new MongoClient(clientSettings);
            }
            Database = _client.GetDatabase(mongoSettings.DatabaseName);
        }

        public IMongoCollection<CustomerGroup> CustomerGroups => Database.GetCollection<CustomerGroup>("merchant.CustomerGroups");
        public IMongoCollection<CustomerEntity> Customers => Database.GetCollection<CustomerEntity>("merchant.Customers");
    }
}
