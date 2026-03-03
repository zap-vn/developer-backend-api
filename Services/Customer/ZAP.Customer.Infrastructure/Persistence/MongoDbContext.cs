using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Customer.Infrastructure.Persistence.Configurations;

namespace ZAP.Customer.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Add Collections as properties here, e.g.:
        // public IMongoCollection<CustomerEntity> Customers => _database.GetCollection<CustomerEntity>("Customers");
    }
}
