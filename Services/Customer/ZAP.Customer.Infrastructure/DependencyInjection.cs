using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ZAP.Customer.Infrastructure.Persistence;
using ZAP.Customer.Infrastructure.Persistence.Configurations;
using ZAP.Customer.Domain.Interfaces;
using ZAP.Customer.Infrastructure.Persistence.Repositories;

namespace ZAP.Customer.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            try 
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            }
            catch (BsonSerializationException) { }

            services.Configure<MongoSettings>(configuration.GetSection("MongoDB"));
            services.AddScoped<ICustomerGroupRepository, CustomerGroupRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<MongoDbContext>();

            return services;
        }
    }
}
