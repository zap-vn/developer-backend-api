using Microsoft.Extensions.DependencyInjection;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Services;

namespace ZAP.BuildingBlocks
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBuildingBlocks(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ICurrentUserService, MockCurrentUserService>();

            // Cấu hình Response JSON trả về chữ cái đầu in hoa (PascalCase) cho tất cả API (Minimal & MVC)
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
