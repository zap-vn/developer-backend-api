using Microsoft.Extensions.DependencyInjection;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Services;

namespace CRM.BuildingBlocks
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBuildingBlocks(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ISystemLanguageProvider, SystemLanguageProvider>();
            services.AddScoped<ISystemErrorProvider, SystemErrorProvider>();
            services.AddScoped<ICurrentUserService, MockCurrentUserService>();

            // Configure JSON Response to return PascalCase for all APIs (Minimal & MVC)
            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = null;
                options.SerializerOptions.DictionaryKeyPolicy = null;
            });

            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });
            
            return services;
        }

        public static IServiceCollection AddBackgroundQueue(this IServiceCollection services, int capacity = 100)
        {
            services.AddSingleton<IBackgroundTaskQueue>(new BackgroundTaskQueue(capacity));
            services.AddHostedService<QueuedHostedService>();
            
            return services;
        }
    }
}
