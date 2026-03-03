using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ZAP.Authentication.Domain.Interfaces;
using ZAP.Authentication.Application.Common.Interfaces;
using ZAP.Authentication.Infrastructure.Persistence;
using ZAP.Authentication.Infrastructure.Persistence.Configurations;
using ZAP.Authentication.Infrastructure.Persistence.Repositories;
using ZAP.Authentication.Infrastructure.Security;

namespace ZAP.Authentication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Guid Serializer globally for this process
            try 
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            }
            catch (BsonSerializationException) 
            {
                // Serializer might already be registered
            }

            services.Configure<MongoSettings>(configuration.GetSection("MongoDB"));

            services.AddSingleton<MongoDbContext>();
            services.AddScoped<IUserRepository, MongoUserRepository>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}
