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
            
            return services;
        }
    }
}
