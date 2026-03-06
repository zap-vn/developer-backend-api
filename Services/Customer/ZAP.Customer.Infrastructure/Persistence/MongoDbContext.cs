using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Customer.Domain.Entities;
using ZAP.Customer.Infrastructure.Persistence.Configurations;

namespace ZAP.Customer.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<CustomerGroup> CustomerGroups => Database.GetCollection<CustomerGroup>("CustomerGroups");
        public IMongoCollection<CustomerEntity> Customers => Database.GetCollection<CustomerEntity>("Customer");
    }
}
