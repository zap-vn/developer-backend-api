using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ZAP.Organization.Infrastructure.Persistence;
using ZAP.Organization.Infrastructure.Persistence.Configurations;

namespace ZAP.Organization.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            try { BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard)); }
            catch (BsonSerializationException) { }

            services.Configure<MongoSettings>(configuration.GetSection("MongoDB"));
            services.AddSingleton<MongoDbContext>();

            return services;
        }
    }
}
