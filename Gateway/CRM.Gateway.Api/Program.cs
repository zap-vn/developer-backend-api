using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CRM.BuildingBlocks;
using CRM.BuildingBlocks.Middleware;
using CRM.Gateway.Api.Configurations;

namespace CRM.Gateway.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Database registration (Required for LocalizationMiddleware)
            builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
            builder.Services.AddSingleton<IMongoDatabase>(sp => 
            {
                var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
                var client = new MongoClient(settings.ConnectionString);
                return client.GetDatabase(settings.DatabaseName);
            });

            builder.Services.AddBuildingBlocks();

            // Configure console logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            Console.WriteLine("🚀 Starting CRM.Gateway.Api...");

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Services
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new CRM.BuildingBlocks.Serialization.CrmSnakeCaseNamingPolicy();
                    options.JsonSerializerOptions.DictionaryKeyPolicy = new CRM.BuildingBlocks.Serialization.CrmSnakeCaseNamingPolicy();
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CRM Gateway API",
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM CONNECT GATEWAY - REALTIME");
                c.SwaggerEndpoint("/api/Auth/swagger/v1/swagger.json", "Authentication Service");
                c.SwaggerEndpoint("/api/Employees/swagger/v1/swagger.json", "HR Service");
                c.SwaggerEndpoint("/api/Customer/swagger/v1/swagger.json", "Customer Service");
                c.SwaggerEndpoint("/api/Sales/swagger/v1/swagger.json", "Sales Service");
                c.SwaggerEndpoint("/api/Products/swagger/v1/swagger.json", "Product Service");
                c.SwaggerEndpoint("/api/Orders/swagger/v1/swagger.json", "Order Service");
                c.SwaggerEndpoint("/api/Payments/swagger/v1/swagger.json", "Payment Service");
                c.SwaggerEndpoint("/api/Organizations/swagger/v1/swagger.json", "Organization Service");
                c.SwaggerEndpoint("/api/Reports/swagger/v1/swagger.json", "Report Service");
                c.SwaggerEndpoint("/api/Promotions/swagger/v1/swagger.json", "Promotion Service");
                
                c.RoutePrefix = string.Empty; // Magic: Show this at http://localhost:5000/
            });

            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.UseMiddleware<LocalizationMiddleware>();

            app.MapReverseProxy();
            app.MapControllers();

            Console.WriteLine("✅ Gateway is now running. Listening for requests...");
            Console.WriteLine("👉 Swagger UI: http://localhost:5000/index.html");
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
