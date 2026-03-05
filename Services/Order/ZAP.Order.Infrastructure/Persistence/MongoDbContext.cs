using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Order.Domain.Entities;
using ZAP.Order.Infrastructure.Persistence.Configurations;

namespace ZAP.Order.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<OrderEntity> Orders => Database.GetCollection<OrderEntity>("Orders");
    }
}
