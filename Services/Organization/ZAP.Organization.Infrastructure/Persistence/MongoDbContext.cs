using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Organization.Domain.Entities;
using ZAP.Organization.Infrastructure.Persistence.Configurations;

namespace ZAP.Organization.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<OrganizationUnit> OrganizationUnits => Database.GetCollection<OrganizationUnit>("OrganizationUnits");
    }
}
