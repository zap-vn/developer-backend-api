using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LegacyDB.Bson;
using LegacyDB.Bson.Serialization;
using LegacyDB.Bson.Serialization.Serializers;
using LegacyDB.Driver;
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

            services.Configure<LegacySettings>(configuration.GetSection("LegacyDB"));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<ZaloSettings>(configuration.GetSection("ZaloSettings"));
            services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));
            services.Configure<VietGuySettings>(configuration.GetSection("VietGuySettings"));

            services.AddHttpClient();
            
            // PostgreSQL
            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));
            
            services.AddSingleton<LegacyDbContext>();
            services.AddSingleton<ILegacyDatabase>(sp => sp.GetRequiredService<LegacyDbContext>().Database);
            
            services.AddScoped<IUserRepository, PostgresUserRepository>();
            services.AddScoped<IPasswordResetRepository, LegacyPasswordResetRepository>();
            services.AddScoped<IOtpRepository, LegacyOtpRepository>();
            services.AddScoped<ISystemConfigRepository, LegacySystemConfigRepository>();
            services.AddScoped<IEmailSettingRepository, LegacyEmailSettingRepository>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IVietGuyService, VietGuyService>();
            services.AddScoped<IPhoneService, PhoneService>();

            return services;
        }
    }
}
