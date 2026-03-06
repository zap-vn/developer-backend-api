using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ZAP.Promotion.Domain.Interfaces;
using ZAP.Promotion.Infrastructure.Persistence;
using ZAP.Promotion.Infrastructure.Persistence.Configurations;
using ZAP.Promotion.Infrastructure.Persistence.Repositories;

namespace ZAP.Promotion.Infrastructure
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
