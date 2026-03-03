using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ZAP.Authentication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            // Add FluentValidation here if needed
            return services;
        }
    }
}
