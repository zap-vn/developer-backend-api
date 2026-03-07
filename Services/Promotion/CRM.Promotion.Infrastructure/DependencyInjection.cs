using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using CRM.Promotion.Domain.Interfaces;
using CRM.Promotion.Infrastructure.Persistence;
using CRM.Promotion.Infrastructure.Persistence.Configurations;
using CRM.Promotion.Infrastructure.Persistence.Repositories;

namespace CRM.Promotion.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));

            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            services.AddScoped<IPromotionRepository, PromotionRepository>();

            return services;
        }
    }
}
