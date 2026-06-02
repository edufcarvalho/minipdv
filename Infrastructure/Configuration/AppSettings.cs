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
    public bool TrustServerCertificate { get; }
    public string LogPath { get; }

    public string ConnectionString =>
        $"Server={DbServer};Database={DbName};User Id={DbUser};Password={DbPassword};TrustServerCertificate={TrustServerCertificate};";

    public AppSettings()
    {
        DbServer = EnvConfig.Get("DB_SERVER") ?? throw new InvalidOperationException("DB_SERVER environment variable is required");
        DbName = EnvConfig.Get("DB_NAME") ?? throw new InvalidOperationException("DB_NAME environment variable is required");
        DbUser = EnvConfig.Get("DB_USER") ?? throw new InvalidOperationException("DB_USER environment variable is required");
        DbPassword = EnvConfig.Get("DB_PASSWORD") ?? throw new InvalidOperationException("DB_PASSWORD environment variable is required");
        JwtSecret = EnvConfig.Get("JWT_SECRET") ?? throw new InvalidOperationException("JWT_SECRET environment variable is required");
        JwtIssuer = EnvConfig.Get("JWT_ISSUER") ?? "MiniPDV";
        JwtAudience = EnvConfig.Get("JWT_AUDIENCE") ?? "MiniPDV";
        JwtExpirationDays = int.TryParse(EnvConfig.Get("JWT_EXPIRATION_DAYS"), out var days) && days > 0 ? days : 1;
        TrustServerCertificate = bool.TryParse(EnvConfig.Get("DB_TRUST_SERVER_CERTIFICATE"), out var trust) && trust;
        LogPath = EnvConfig.Get("LOG_PATH") ?? "logs";
    }
}
