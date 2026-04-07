using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CRM.Product.Domain.Interfaces;
using CRM.Product.Infrastructure.Persistence;
using CRM.Product.Infrastructure.Persistence.Repositories;

namespace CRM.Product.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgreSql") ?? 
                                  configuration["ConnectionStrings:PostgreSql"];
                                  
            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString)
                       .EnableSensitiveDataLogging()
                       .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

            services.AddScoped<IProductRepository, PostgresProductRepository>();
            services.AddScoped<ICategoryRepository, PostgresCategoryRepository>();
            services.AddScoped<IBrandRepository, PostgresBrandRepository>();
            services.AddScoped<IModifierGroupRepository, PostgresModifierGroupRepository>();
            services.AddScoped<IUnitRepository, PostgresUnitRepository>();
            services.AddScoped<ILocationRepository, PostgresLocationRepository>();

            return services;
        }
    }
}

