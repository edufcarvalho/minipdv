using dotenv.net;

namespace minipdv.Infrastructure.Configuration;

public static class EnvConfig
{
    private static IDictionary<string, string>? _dotEnv;
    private static readonly object _lock = new();

    public static string? Get(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrEmpty(value))
            return value;

        EnsureDotEnvLoaded();
        if (_dotEnv is not null && _dotEnv.TryGetValue(key, out var dotEnvValue))
            return dotEnvValue;

        return null;
    }

    private static void EnsureDotEnvLoaded()
    {
        if (_dotEnv is not null)
            return;

        lock (_lock)
        {
            if (_dotEnv is not null)
                return;

            var path = ResolvePath();
            if (path is not null)
                _dotEnv = DotEnv.Read(new DotEnvOptions(envFilePaths: [path], ignoreExceptions: true));
        }
    }

    private static string? ResolvePath()
    {
        string[] candidates =
        [
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(AppContext.BaseDirectory, ".env")
        ];

        return candidates.FirstOrDefault(File.Exists);
    }
}
