using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using CRM.Order.Domain.Interfaces;
using CRM.Order.Infrastructure.Persistence;
using CRM.Order.Infrastructure.Persistence.Configurations;
using CRM.Order.Infrastructure.Persistence.Repositories;

namespace CRM.Order.Infrastructure
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
