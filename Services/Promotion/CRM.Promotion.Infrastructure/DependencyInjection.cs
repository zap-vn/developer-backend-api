using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using CRM.Promotion.Domain.Interfaces;
using CRM.Promotion.Infrastructure.Persistence;
using CRM.Promotion.Infrastructure.Persistence.Configurations;
using CRM.Promotion.Infrastructure.Persistence.Repositories;
using System.Reflection;

namespace CRM.Promotion.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // MongoDB (write side)
            services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));
            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);
            services.AddScoped<IPromotionRepository, PromotionRepository>();

            // PostgreSQL (read side)
            var connectionString = configuration.GetConnectionString("PostgreSql") ??
                                   configuration["ConnectionStrings:PostgreSql"];
            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString)
                       .EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));


            return services;
        }
    }
}
