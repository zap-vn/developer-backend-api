using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

Console.WriteLine("--> Zap Gateway API Starting...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Zap Gateway API",
            Version = "v1",
            Description = "Public Gateway for Zap Platform"
        });
    });

    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    // Add JWT Authentication
    var jwtSecret = builder.Configuration["JwtSettings:Secret"];
    var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "Zap.Identity.Api";
    var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "Zap.Client";

    if (string.IsNullOrEmpty(jwtSecret))
    {
         Console.WriteLine("--> WARNING: JwtSettings:Secret is null or empty! JWT Validation might fail or be insecure.");
         // Fallback for development if needed, or just let it fail later
         jwtSecret = "default_secret_key_must_be_long_enough_for_gateway_to_validate"; 
    }
    else
    {
         Console.WriteLine($"--> JwtSettings loaded. Issuer: {jwtIssuer}, Secret Length: {jwtSecret.Length}");
    }

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Option 1: Use Authority (if Identity API supports OIDC discovery)
        // options.Authority = builder.Configuration["Jwt:Authority"];
        // options.RequireHttpsMetadata = false;

        // Option 2: Manual Validation (Matching Identity.Api logic)
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Permissive for dev
            ValidateAudience = false, // Permissive for dev
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true, // Gateway MUST validate signature
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };

        // Events for debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"--> Auth Failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"--> Token Validated: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Public", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
    });

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());
    });

    var app = builder.Build();

    // Global Error Handling
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
            throw; // Re-throw to let YARP or default handler catch if needed, or handle response here
        }
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API");
             // Optional: Keep reference to Identity API Swagger if reachable
             // c.SwaggerEndpoint("...", "Identity API");
        });
    }

    app.UseRouting();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    
    // Health Check
    app.MapGet("/health", () => "Gateway OK");

    app.MapReverseProxy();

    Console.WriteLine("--> Zap Gateway API built successfully. Running...");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"--> CRITICAL ERROR: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}
finally
{
    Console.WriteLine("--> Zap Gateway API Exiting...");
}
