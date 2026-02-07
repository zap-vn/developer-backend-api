namespace Zap.Identity.Infrastructure.Persistence;

public class DatabaseSettings
{
    public const string SectionName = "DatabaseSettings";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
