using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ZAP.Order.Domain.Interfaces;
using ZAP.Order.Infrastructure.Persistence;
using ZAP.Order.Infrastructure.Persistence.Configurations;
using ZAP.Order.Infrastructure.Persistence.Repositories;

namespace ZAP.Order.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));

            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
