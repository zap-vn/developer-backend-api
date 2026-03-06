using ZAP.BuildingBlocks;
using ZAP.BackgroundWorker.Application;
using ZAP.BackgroundWorker.Infrastructure;
using ZAP.BuildingBlocks.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Shared Building Blocks
builder.Services.AddBuildingBlocks();
builder.Services.AddBackgroundQueue(capacity: 100);

// Product Layer Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<LocalizationMiddleware>();

app.MapControllers();

app.Run();
