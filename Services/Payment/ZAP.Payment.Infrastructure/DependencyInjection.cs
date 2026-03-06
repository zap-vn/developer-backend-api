using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ZAP.Payment.Domain.Interfaces;
using ZAP.Payment.Infrastructure.Persistence;
using ZAP.Payment.Infrastructure.Persistence.Configurations;
using ZAP.Payment.Infrastructure.Persistence.Repositories;

namespace ZAP.Payment.Infrastructure
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
