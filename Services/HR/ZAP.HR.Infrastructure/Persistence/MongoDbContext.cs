using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.HR.Domain.Entities;
using ZAP.HR.Infrastructure.Persistence.Configurations;

namespace ZAP.HR.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Employee> Employees => Database.GetCollection<Employee>("Employees");
        public IMongoCollection<EmployeeTranslation> EmployeeTranslations => Database.GetCollection<EmployeeTranslation>("Employee_translate");
    }
}
