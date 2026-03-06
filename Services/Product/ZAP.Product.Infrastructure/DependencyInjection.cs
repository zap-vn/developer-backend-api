using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ZAP.Product.Domain.Interfaces;
using ZAP.Product.Infrastructure.Persistence;
using ZAP.Product.Infrastructure.Persistence.Configurations;
using ZAP.Product.Infrastructure.Persistence.Repositories;

namespace ZAP.Product.Infrastructure
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
