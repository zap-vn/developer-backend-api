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

            // Configure JSON Response to return PascalCase for all APIs (Minimal & MVC), except _id
            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = new Serialization.ExceptionPascalCaseNamingPolicy();
                options.SerializerOptions.DictionaryKeyPolicy = new Serialization.ExceptionPascalCaseNamingPolicy();
            });

            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new Serialization.ExceptionPascalCaseNamingPolicy();
                options.JsonSerializerOptions.DictionaryKeyPolicy = new Serialization.ExceptionPascalCaseNamingPolicy();
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
