using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ZAP.Organization.Domain.Interfaces;
using ZAP.Organization.Infrastructure.Persistence;
using ZAP.Organization.Infrastructure.Persistence.Configurations;
using ZAP.Organization.Infrastructure.Persistence.Repositories;

namespace ZAP.Organization.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));

            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            services.AddScoped<IOrganizationRepository, OrganizationRepository>();

            return services;
        }
    }
}
