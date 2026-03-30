using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CRM.Order.Domain.Entities;
using CRM.Order.Infrastructure.Persistence.Configurations;

namespace CRM.Order.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<OrderEntity> Orders => Database.GetCollection<OrderEntity>("ordering.Orders");
    }
}
