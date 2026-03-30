using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CRM.HR.Domain.Entities;
using CRM.HR.Infrastructure.Persistence.Configurations;

namespace CRM.HR.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Employee> Employees => Database.GetCollection<Employee>("merchant.Employees");
        public IMongoCollection<EmployeeTranslation> EmployeeTranslations => Database.GetCollection<EmployeeTranslation>("merchant.Employee_translate");
    }
}
