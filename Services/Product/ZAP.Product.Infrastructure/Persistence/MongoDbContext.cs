using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Product.Domain.Entities;
using ZAP.Product.Infrastructure.Persistence.Configurations;

namespace ZAP.Product.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<ProductEntity> Products => _database.GetCollection<ProductEntity>("Products");
    }
}
