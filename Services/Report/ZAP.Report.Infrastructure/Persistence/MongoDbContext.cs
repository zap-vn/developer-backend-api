using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Report.Domain.Entities;
using ZAP.Report.Infrastructure.Persistence.Configurations;

namespace ZAP.Report.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<ReportTemplate> ReportTemplates => Database.GetCollection<ReportTemplate>("ReportTemplates");
    }
}
