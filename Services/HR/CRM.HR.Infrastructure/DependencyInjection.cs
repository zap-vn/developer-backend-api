using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using CRM.HR.Domain.Interfaces;
using CRM.HR.Infrastructure.Persistence;
using CRM.HR.Infrastructure.Persistence.Configurations;
using CRM.HR.Infrastructure.Persistence.Repositories;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Services;

namespace CRM.HR.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Guid Serializer
            try 
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            }
            catch (BsonSerializationException) 
            {
                // Already registered
            }

            services.Configure<MongoSettings>(configuration.GetSection("MongoDB"));

            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            services.AddScoped<IEmployeeRepository, MongoEmployeeRepository>();
            services.AddScoped<ICurrentUserService, CRM.BuildingBlocks.Services.CurrentUserService>();

            return services;
        }
    }
}
