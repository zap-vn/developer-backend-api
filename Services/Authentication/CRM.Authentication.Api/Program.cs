using CRM.BuildingBlocks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CRM.Authentication.Application;
using CRM.Authentication.Infrastructure;
using CRM.BuildingBlocks.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new CRM.BuildingBlocks.Serialization.ExceptionPascalCaseNamingPolicy();
        options.JsonSerializerOptions.DictionaryKeyPolicy = new CRM.BuildingBlocks.Serialization.ExceptionPascalCaseNamingPolicy();
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLocalization();
builder.Services.AddMemoryCache();
builder.Services.AddBackgroundQueue(100);
builder.Services.AddHttpClient();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// TTL Application Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSingleton<CRM.Authentication.Infrastructure.Security.OtpService>();

// Authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "a_very_secret_default_key_at_least_32_chars_long_1234567890"))
        };
    });

builder.Services.AddBuildingBlocks();
builder.Services.AddHealthChecks(); // Register health checks

try {
    var app = builder.Build();

    app.UseMiddleware<LocalizationMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseResponseCompression();
    app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API v1");
            c.RoutePrefix = "swagger"; // Explicitly set or keep empty, but let's see why it failed.
        });

    app.UseRouting();

    app.UseCors("AllowAll");

    // app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    // Duplicate LocalizationMiddleware removed
    app.MapHealthChecks("/healthz");
    app.MapControllers();

    Console.WriteLine("✅ Authentication API is running.");
    Console.WriteLine("👉 Local URL: http://localhost:5001/swagger/index.html");
    app.Run();
} catch (Exception ex) {
    Console.Error.WriteLine("FATAL ERROR DURING STARTUP:");
    Console.Error.WriteLine(ex.ToString());
    throw;
}
