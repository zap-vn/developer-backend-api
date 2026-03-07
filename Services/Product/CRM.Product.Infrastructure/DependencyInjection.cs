using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using CRM.Product.Domain.Interfaces;
using CRM.Product.Infrastructure.Persistence;
using CRM.Product.Infrastructure.Persistence.Configurations;
using CRM.Product.Infrastructure.Persistence.Repositories;

namespace CRM.Product.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));

            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
