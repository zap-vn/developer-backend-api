using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using CRM.Customer.Infrastructure.Persistence;
using CRM.Customer.Infrastructure.Persistence.Configurations;
using CRM.Customer.Domain.Interfaces;
using CRM.Customer.Infrastructure.Persistence.Repositories;

namespace CRM.Customer.Infrastructure
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
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            return services;
        }
    }
}
