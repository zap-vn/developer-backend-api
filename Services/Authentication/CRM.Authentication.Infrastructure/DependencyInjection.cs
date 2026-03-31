using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CRM.Authentication.Domain.Interfaces;
using CRM.Authentication.Application.Common.Interfaces;
using CRM.Authentication.Application.Common.Models;
using CRM.Authentication.Infrastructure.Persistence;
using CRM.Authentication.Infrastructure.Persistence.Repositories;
using CRM.Authentication.Infrastructure.Security;

namespace CRM.Authentication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<ZaloSettings>(configuration.GetSection("ZaloSettings"));
            services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));
            services.Configure<VietGuySettings>(configuration.GetSection("VietGuySettings"));

            services.AddHttpClient();
            
            // PostgreSQL Database Context
            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));
            
            // Repositories (Unified on PostgreSQL via IUserRepository)
            services.AddScoped<IUserRepository, PostgresUserRepository>();
            services.AddScoped<IPasswordResetRepository, LegacyPasswordResetRepository>();
            services.AddScoped<IOtpRepository, LegacyOtpRepository>();
            services.AddScoped<ISystemConfigRepository, LegacySystemConfigRepository>();
            services.AddScoped<IEmailSettingRepository, LegacyEmailSettingRepository>();
            
            // Infrastructure Services
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IVietGuyService, VietGuyService>();
            services.AddScoped<IPhoneService, PhoneService>();

            return services;
        }
    }
}
