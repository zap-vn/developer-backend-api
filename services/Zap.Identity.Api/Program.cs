using Microsoft.OpenApi.Models;
using Zap.Identity.Infrastructure;

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
    });

    // Add Infrastructure services (MongoDB, Repositories, AuthService)
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());
    });

    var app = builder.Build();

    app.UseCors("AllowAll"); // Enable CORS before Swagger and Auth

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else 
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthorization();

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
