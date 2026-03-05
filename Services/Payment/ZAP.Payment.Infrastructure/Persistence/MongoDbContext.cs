using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Payment.Domain.Entities;
using ZAP.Payment.Infrastructure.Persistence.Configurations;

namespace ZAP.Payment.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<PaymentMethod> PaymentMethods => Database.GetCollection<PaymentMethod>("PaymentMethods");
    }
}
