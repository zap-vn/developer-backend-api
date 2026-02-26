using Microsoft.OpenApi.Models;
using NSwag.Annotations;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zap.Identity.Infrastructure;
using Zap.Identity.Infrastructure.Settings;
using Zap.Identity.Api.Middleware;


Console.WriteLine("--> Zap Identity API Starting...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Zap Identity API",
            Version = "v1",
            Description = "Authentication and Customer Management API for Zap Platform"
        });
        
        // Add Bearer Token Support
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer {your_token}'"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });

        // Sắp xếp các API (Actions) theo Controller/Path
        c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");

        // Add support for NSwag OpenApiOperation attribute in Swashbuckle
        c.OperationFilter<NSwagOperationFilter>();
    });





    // Add Infrastructure services (MongoDB, Repositories, AuthService)
    try 
    {
        builder.Services.AddInfrastructureServices(builder.Configuration);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> WARNING: Failed to initialize Infrastructure Services (Database): {ex.Message}");
        // Continuing startup to allow Swagger UI to load even without DB
    }

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
            ValidateLifetime = false, // Temporarily disabled for dev integration
            ValidateIssuerSigningKey = false,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret ?? "default_secret_key_must_be_long_enough")),
            // Completely bypass signature validation for legacy tokens
            SignatureValidator = (token, parameters) => new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token)
        };
    });
    
    builder.Services.AddAuthorization();

    // Add CORS chưa có
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());
    });

    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();

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
    app.UseRouting();// thay đổi ngay 2026-02-13
    app.UseCors("AllowAll");
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

public class NSwagOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
    public void Apply(OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
    {
        var nswagAttribute = context.MethodInfo
            .GetCustomAttributes(typeof(OpenApiOperationAttribute), true)
            .FirstOrDefault() as OpenApiOperationAttribute;

        if (nswagAttribute != null)
        {
            operation.Summary = nswagAttribute.Summary;
            operation.Description = nswagAttribute.Description;
        }
    }
}

