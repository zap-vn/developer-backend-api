using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ZAP.Report.Domain.Interfaces;
using ZAP.Report.Infrastructure.Persistence;
using ZAP.Report.Infrastructure.Persistence.Configurations;
using ZAP.Report.Infrastructure.Persistence.Repositories;

namespace ZAP.Report.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoSettings>(configuration.GetSection("MongoSettings"));

            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<MongoDbContext>().Database);

            services.AddScoped<IReportRepository, ReportRepository>();

            return services;
        }
    }
}
