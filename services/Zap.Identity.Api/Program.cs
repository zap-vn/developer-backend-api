using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zap.Identity.Infrastructure;
using Zap.Identity.Infrastructure.Settings;

Console.WriteLine("--> Zap Identity API Starting...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Zap Identity API",
            Version = "v1",
            Description = "Authentication API for Zap Platform"
        });
        // Sắp xếp các API (Actions) theo Controller/Path
        c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");
    });

    // Add Infrastructure services (MongoDB, Repositories, AuthService)
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Add JWT Authentication
    var jwtSettings = new JwtSettings();
    builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
    
    if (string.IsNullOrEmpty(jwtSettings.Secret))
    {
        Console.WriteLine("--> ERROR: JwtSettings.Secret is null or empty!");
    }
    else
    {
        Console.WriteLine($"--> JwtSettings loaded. Issuer: {jwtSettings.Issuer}, Secret Length: {jwtSettings.Secret.Length}");
    }

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // WARNING: Signature validation disabled for development/testing
            // This allows legacy tokens to work without the original JWT secret
            // TODO: Re-enable for production or after migrating to new token system
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,  // Temporarily disabled for legacy token support
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret ?? "default_secret_key_must_be_long_enough"))
        };
    });
    
    builder.Services.AddAuthorization();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());
    });

    var app = builder.Build();

    app.Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> ERROR: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    });

    app.UseCors("AllowAll");

    // Enable Swagger early in the pipeline
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zap Identity API v1");
        c.RoutePrefix = string.Empty; // Fail-safe: Serve Swagger at root
    });

    if (!app.Environment.IsDevelopment())
    {
        // app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    // Simple health check
    app.MapGet("/health", () => "OK");

    app.MapControllers();

    Console.WriteLine("--> App built successfully. Running...");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"--> CRITICAL ERROR: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}
finally
{
    Console.WriteLine("--> Zap Identity API Exiting...");
}
