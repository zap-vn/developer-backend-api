using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CRM.Promotion.Domain.Entities;
using CRM.Promotion.Infrastructure.Persistence.Configurations;

namespace CRM.Promotion.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<PromotionEntity> Promotions => Database.GetCollection<PromotionEntity>("Promotions");
    }
}
