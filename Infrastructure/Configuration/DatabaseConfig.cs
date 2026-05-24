using dotenv.net;

namespace minipdv.Infrastructure.Configuration;

public class DatabaseConfig
{
    public string Server { get; set; } = @"localhost\SQLEXPRESS";
    public string Database { get; set; } = "SQLMINIPDV";
    public string User { get; set; } = "sa";
    public string Password { get; set; } = "";

    public string ConnectionString =>
        $"Server={Server};Database={Database};User Id={User};Password={Password};TrustServerCertificate=True;";

    public static DatabaseConfig FromEnv(string envFilePath)
    {
        var env = DotEnv.Read(new DotEnvOptions(envFilePaths: [envFilePath], ignoreExceptions: true));

        return new DatabaseConfig
        {
            Server = GetValue(env, "DB_SERVER", @"localhost\SQLEXPRESS"),
            Database = GetValue(env, "DB_NAME", "SQLMINIPDV"),
            User = GetValue(env, "DB_USER", "sa"),
            Password = GetValue(env, "DB_PASSWORD", "")
        };
    }

    private static string GetValue(IDictionary<string, string> env, string key, string defaultValue)
    {
        return env.TryGetValue(key, out var value) ? value : defaultValue;
    }
}
