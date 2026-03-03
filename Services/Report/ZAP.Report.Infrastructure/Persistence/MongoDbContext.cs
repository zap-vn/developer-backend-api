using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Report.Infrastructure.Persistence.Configurations;

namespace ZAP.Report.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }
    }
}
