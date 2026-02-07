using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


namespace Zap.Identity.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Persistence.DatabaseSettings>(configuration.GetSection("DatabaseSettings"));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<Persistence.DatabaseSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<Persistence.DatabaseSettings>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        return services;
    }
}
