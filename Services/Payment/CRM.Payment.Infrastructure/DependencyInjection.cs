using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using CRM.Payment.Domain.Interfaces;
using CRM.Payment.Infrastructure.Persistence;
using CRM.Payment.Infrastructure.Persistence.Configurations;
using CRM.Payment.Infrastructure.Persistence.Repositories;

namespace CRM.Payment.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));

            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
            services.AddScoped<IPaymentTermsRepository, PaymentTermsRepository>();

            return services;
        }
    }
}
