using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using CRM.Organization.Domain.Interfaces;
using CRM.Organization.Infrastructure.Persistence;
using CRM.Organization.Infrastructure.Persistence.Configurations;
using CRM.Organization.Infrastructure.Persistence.Repositories;

namespace CRM.Organization.Infrastructure
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
