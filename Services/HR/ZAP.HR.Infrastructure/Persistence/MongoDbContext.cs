using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.HR.Domain.Entities;
using ZAP.HR.Infrastructure.Persistence.Configurations;

namespace ZAP.HR.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Employee> Employees => _database.GetCollection<Employee>("Employees");
        public IMongoCollection<EmployeeTranslation> EmployeeTranslations => _database.GetCollection<EmployeeTranslation>("Employee_translate");
    }
}
