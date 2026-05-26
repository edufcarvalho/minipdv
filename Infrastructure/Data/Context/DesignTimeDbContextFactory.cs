using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using minipdv.Infrastructure.Configuration;

namespace minipdv.Infrastructure.Data.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MiniPDVContext>
{
    public MiniPDVContext CreateDbContext(string[] args)
    {
        var envPath = ResolveEnvPath();
        var config = envPath is not null ? DatabaseConfig.FromEnv(envPath) : new DatabaseConfig();

        var options = new DbContextOptionsBuilder<MiniPDVContext>()
            .UseSqlServer(config.ConnectionString)
            .Options;

        return new MiniPDVContext(options);
    }

    private static string? ResolveEnvPath()
    {
        string[] candidates =
        [
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(AppContext.BaseDirectory, ".env")
        ];

        return candidates.FirstOrDefault(File.Exists);
    }
}
