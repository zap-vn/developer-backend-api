using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ZAP.Report.Infrastructure.Persistence;
using ZAP.Report.Infrastructure.Persistence.Configurations;

namespace ZAP.Report.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            try { BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard)); }
            catch (BsonSerializationException) { }

            services.Configure<MongoSettings>(configuration.GetSection("MongoDB"));
            services.AddSingleton<MongoDbContext>();
            services.AddScoped<ZAP.Report.Application.Common.Interfaces.IReportRepository, ZAP.Report.Infrastructure.Repositories.ReportRepository>();

            return services;
        }
    }
}
