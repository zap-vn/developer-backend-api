using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ZAP.HR.Domain.Interfaces;
using ZAP.HR.Infrastructure.Persistence;
using ZAP.HR.Infrastructure.Persistence.Configurations;
using ZAP.HR.Infrastructure.Persistence.Repositories;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Services;

namespace ZAP.HR.Infrastructure
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

            services.AddScoped<IEmployeeRepository, MongoEmployeeRepository>();
            services.AddScoped<ICurrentUserService, MockCurrentUserService>();

            return services;
        }
    }
}
