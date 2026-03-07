using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CRM.Payment.Domain.Entities;
using CRM.Payment.Infrastructure.Persistence.Configurations;

namespace CRM.Payment.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<PaymentType> PaymentTypes => Database.GetCollection<PaymentType>("PaymentTypes");
        public IMongoCollection<PaymentTerms> PaymentTerms => Database.GetCollection<PaymentTerms>("PaymentTerms");
    }
}
