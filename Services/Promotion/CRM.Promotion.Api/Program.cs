using CRM.BuildingBlocks;
using CRM.Promotion.Application;
using CRM.Promotion.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
// builder.Services.AddOpenApi();

builder.Services.AddBuildingBlocks();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// Register MediatR for all assemblies
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(Program).Assembly,
    typeof(CRM.Promotion.Application.DependencyInjection).Assembly,
    typeof(CRM.Promotion.Infrastructure.DependencyInjection).Assembly
));

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CRM Promotion API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM Promotion API V1");
    });
}

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<CRM.BuildingBlocks.Middleware.LocalizationMiddleware>();

try
{
    Console.WriteLine("🚀 CRM Promotion API is starting...");
    app.Run();
}
catch (Exception ex)
{
    Console.Error.WriteLine("FATAL ERROR DURING PROMOTION API STARTUP:");
    Console.Error.WriteLine(ex.ToString());
    throw;
}
