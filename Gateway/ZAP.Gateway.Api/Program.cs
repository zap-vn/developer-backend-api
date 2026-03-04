using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZAP.BuildingBlocks.Middleware;

namespace ZAP.Gateway.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure console logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            Console.WriteLine("🚀 Starting ZAP.Gateway.Api...");

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Services
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ZAP Gateway API",
        Version = "v1"
    });
});

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", b =>
                {
                    b.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            app.UseResponseCompression();

            // Minimal health‑check endpoint
            app.MapGet("/health", () => Results.Ok("Gateway is healthy"));

            // Enable static files for index.html
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Provide the swagger.json
            app.UseSwagger();
            
            // This is the MOST RELIABLE way to show the dropdown
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZAP CONNECT GATEWAY - REALTIME");
                c.SwaggerEndpoint("/api/Auth/swagger/v1/swagger.json", "Authentication Service");
                c.SwaggerEndpoint("/api/Employees/swagger/v1/swagger.json", "HR Service");
                c.SwaggerEndpoint("/api/Customer/swagger/v1/swagger.json", "Customer Service");
                c.SwaggerEndpoint("/api/Sales/swagger/v1/swagger.json", "Sales Service");
                c.SwaggerEndpoint("/api/Product/swagger/v1/swagger.json", "Product Service");
                c.SwaggerEndpoint("/api/Order/swagger/v1/swagger.json", "Order Service");
                c.SwaggerEndpoint("/api/Payment/swagger/v1/swagger.json", "Payment Service");
                c.SwaggerEndpoint("/api/Organization/swagger/v1/swagger.json", "Organization Service");
                c.SwaggerEndpoint("/api/Report/swagger/v1/swagger.json", "Report Service");
                
                c.RoutePrefix = string.Empty; // Magic: Show this at http://localhost:5000/
            });

            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.UseMiddleware<LocalizationMiddleware>();

            app.MapReverseProxy();
            app.MapControllers();

            Console.WriteLine("✅ Gateway is now running. Listening for requests...");
            Console.WriteLine(">>> DEBUG: BEFORE RunAsync <<<");
            await app.RunAsync();

            Console.WriteLine(">>> STARTUP END <<<");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[FATAL] Application failed to start: {ex.Message}");
            Console.Error.WriteLine(ex);
        }
    }
}
