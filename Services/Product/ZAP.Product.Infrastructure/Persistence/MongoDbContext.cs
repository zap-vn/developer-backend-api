using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Product.Domain.Entities;
using ZAP.Product.Infrastructure.Persistence.Configurations;

namespace ZAP.Product.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<ProductEntity> Products => Database.GetCollection<ProductEntity>("Products");
    }
}
