using Microsoft.EntityFrameworkCore;
using minipdv.Infrastructure.Configuration;
using minipdv.Infrastructure.Data.Context;
using minipdv.Infrastructure.Data.Seed;
using minipdv.Presentation.Desktop.Forms.Shared;

namespace minipdv;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var runAsApi = args.Contains("--api")
            || Environment.GetEnvironmentVariable("ASPNETCORE_IIS_PHYSICAL_PATH") is not null;

        if (runAsApi)
        {
            RunApiAsync(args).GetAwaiter().GetResult();
        }
        else
        {
            RunWinForms();
        }
    }

    static async Task RunApiAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var envPath = ResolveEnvPath();
        var dbConfig = envPath is not null ? DatabaseConfig.FromEnv(envPath) : new DatabaseConfig();

        builder.Services.AddDbContext<MiniPDVContext>(options =>
            options.UseSqlServer(dbConfig.ConnectionString));

        builder.Services.AddControllers();
        builder.Services.AddSingleton(sp =>
        {
            var context = sp.GetRequiredService<MiniPDVContext>();
            var seedsPath = ResolveSeedsPath();
            return new DatabaseInitializer(context, seedsPath);
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            try
            {
                if (!initializer.IsDatabaseSeeded())
                    await initializer.SeedAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Database seeding skipped: {ex.Message}");
            }
        }

        app.MapControllers();
        await app.RunAsync();
    }

    static void RunWinForms()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            var envPath = ResolveEnvPath();
            if (envPath is not null)
            {
                var config = DatabaseConfig.FromEnv(envPath);
                var options = new DbContextOptionsBuilder<MiniPDVContext>()
                    .UseSqlServer(config.ConnectionString)
                    .Options;
                using var context = new MiniPDVContext(options);
                var seedsPath = ResolveSeedsPath();
                var initializer = new DatabaseInitializer(context, seedsPath);

                if (!initializer.IsDatabaseSeeded())
                    initializer.Seed();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Database seeding failed: {ex.Message}",
                "MiniPDV",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        System.Windows.Forms.Application.Run(new Form1());
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

    private static string ResolveSeedsPath()
    {
        string[] candidates =
        [
            Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "Data", "Seed"),
            Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Data", "Seed"),
            Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seed")
        ];

        return candidates.FirstOrDefault(Directory.Exists)
            ?? Path.Combine(AppContext.BaseDirectory, "Data", "Seed");
    }
}
