using Microsoft.EntityFrameworkCore;
using CRM.Customer.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?> {
        {"ConnectionStrings:PostgreSql", "Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem"}
    })
    .Build();

services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));

var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();

var count = await context.Customers.CountAsync();
Console.WriteLine($"Total Customers in Postgres: {count}");

var tenantId = new Guid("f47ac10b-fa24-4372-a567-607d00000000");
var tenantCount = await context.Customers.CountAsync(c => c.tenant_id == tenantId);
Console.WriteLine($"Customers for tenant {tenantId}: {tenantCount}");
