namespace minipdv.Infrastructure.Configuration;

public class DatabaseConfig
{
    public string Server { get; set; } = "127.0.0.1,1433";
    public string Database { get; set; } = "MINIPDV";
    public string User { get; set; } = "sa";
    public string Password { get; set; } = "MiniPDV@2026!";

    public string ConnectionString =>
        $"Server={Server};Database={Database};User Id={User};Password={Password};TrustServerCertificate=True;";

    public static DatabaseConfig Load()
    {
        return new DatabaseConfig
        {
            Server = EnvConfig.Get("DB_SERVER") ?? "127.0.0.1,1433",
            Database = EnvConfig.Get("DB_NAME") ?? "MINIPDV",
            User = EnvConfig.Get("DB_USER") ?? "sa",
            Password = EnvConfig.Get("DB_PASSWORD") ?? "MiniPDV@2026!"
        };
    }
}
