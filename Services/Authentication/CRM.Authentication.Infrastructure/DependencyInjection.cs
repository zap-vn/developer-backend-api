using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Infrastructure.Persistence;
using CRM.Authentication.Infrastructure.Persistence.Configurations;
using CRM.Authentication.Infrastructure.Persistence.Repositories;
using CRM.Authentication.Infrastructure.Security;

using CRM.Authentication.Application.Common.Models;

namespace CRM.Authentication.Infrastructure
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
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            services.AddSingleton<MongoDbContext>();
            services.AddScoped<IUserRepository, MongoUserRepository>();
            services.AddScoped<IPasswordResetRepository, MongoPasswordResetRepository>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPhoneService, PhoneService>();

            return services;
        }
    }
}
