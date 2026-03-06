using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZAP.Sales.Domain.Entities.Orders;
using ZAP.Sales.Domain.Entities.Products;
using ZAP.Sales.Domain.Entities.Payments;
using ZAP.Sales.Domain.Entities.Organizations;
using ZAP.Sales.Domain.Entities.Reports;
using ZAP.Sales.Domain.Entities.Promotions;
using ZAP.Sales.Infrastructure.Persistence.Configurations;

namespace ZAP.Sales.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Promotion> Promotions => Database.GetCollection<Promotion>("Promotions");
        public IMongoCollection<OrderEntity> Orders => Database.GetCollection<OrderEntity>("Orders");
        public IMongoCollection<ProductEntity> Products => Database.GetCollection<ProductEntity>("Products");
        public IMongoCollection<PaymentMethod> Payments => Database.GetCollection<PaymentMethod>("Payments");
        public IMongoCollection<OrganizationUnit> OrganizationUnits => Database.GetCollection<OrganizationUnit>("OrganizationUnits");
        public IMongoCollection<ReportTemplate> Reports => Database.GetCollection<ReportTemplate>("Reports");
    }
}
