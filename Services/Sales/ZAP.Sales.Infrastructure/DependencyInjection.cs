using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ZAP.Sales.Domain.Interfaces;
using ZAP.Sales.Application.Common.Interfaces;
using ZAP.Sales.Infrastructure.Persistence;
using ZAP.Sales.Infrastructure.Persistence.Configurations;
using ZAP.Sales.Infrastructure.Persistence.Repositories;

namespace ZAP.Sales.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            try { BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard)); }
            catch (BsonSerializationException) { }

            services.Configure<MongoSettings>(configuration.GetSection("MongoDB"));
            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            // Register Repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();

            return services;
        }
    }
}
