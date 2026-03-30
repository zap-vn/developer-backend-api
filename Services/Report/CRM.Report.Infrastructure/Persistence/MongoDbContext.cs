using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CRM.Report.Domain.Entities;
using CRM.Report.Infrastructure.Persistence.Configurations;

namespace CRM.Report.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<ReportTemplate> Reports => Database.GetCollection<ReportTemplate>("report.ReportTemplates");
    }
}
