using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Sales.Domain.Entities;
using ZAP.Sales.Infrastructure.Persistence.Configurations;

namespace ZAP.Sales.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Promotion> Promotions => Database.GetCollection<Promotion>("Promotions");
    }
}
