using CRM.BuildingBlocks;
using CRM.Product.Application;
using CRM.Product.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new CRM.BuildingBlocks.Serialization.CrmSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.DictionaryKeyPolicy = new CRM.BuildingBlocks.Serialization.CrmSnakeCaseNamingPolicy();
    });

// Authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "a_very_secret_default_key_at_least_32_chars_long_1234567890"))
    };
});

builder.Services.AddBuildingBlocks();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// Register MediatR for all assemblies
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(Program).Assembly,
    typeof(CRM.Product.Application.DependencyInjection).Assembly,
    typeof(CRM.Product.Infrastructure.DependencyInjection).Assembly
));

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CRM.Product.Api",
        Version = "1.0"
    });
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM Product API V1");
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try {
    Console.WriteLine("🚀 CRM Product API is starting...");
    app.Run();
} catch (Exception ex) {
    Console.Error.WriteLine("FATAL ERROR DURING PRODUCT API STARTUP:");
    Console.Error.WriteLine(ex.ToString());
    throw;
}
