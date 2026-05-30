namespace minipdv.Infrastructure.Configuration;

public class AppSettings
{
    public string DbServer { get; }
    public string DbName { get; }
    public string DbUser { get; }
    public string DbPassword { get; }
    public string JwtSecret { get; }
    public string JwtIssuer { get; }
    public string JwtAudience { get; }
    public int JwtExpirationDays { get; }

    public string ConnectionString =>
        $"Server={DbServer};Database={DbName};User Id={DbUser};Password={DbPassword};TrustServerCertificate=True;";

    public AppSettings()
    {
        DbServer = EnvConfig.Get("DB_SERVER") ?? "127.0.0.1,1433";
        DbName = EnvConfig.Get("DB_NAME") ?? "MINIPDV";
        DbUser = EnvConfig.Get("DB_USER") ?? "sa";
        DbPassword = EnvConfig.Get("DB_PASSWORD") ?? "MiniPDV@2026!";
        JwtSecret = EnvConfig.Get("JWT_SECRET") ?? "change-in-production-should-be-at-least-32-chars";
        JwtIssuer = "MiniPDV";
        JwtAudience = "MiniPDV";
        JwtExpirationDays = 7;
    }
}
