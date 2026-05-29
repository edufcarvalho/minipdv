using Microsoft.EntityFrameworkCore;
using minipdv.Infrastructure.Configuration;
using minipdv.Infrastructure.Data.Context;
using minipdv.Infrastructure.Data.Seed;
#if WINDOWS
using minipdv.Presentation.Desktop.Forms.Shared;
#endif

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
#if WINDOWS
        else
        {
            RunWinForms();
        }
#else
        else
        {
            RunApiAsync(args).GetAwaiter().GetResult();
        }
#endif
    }

    static async Task RunApiAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var dbConfig = DatabaseConfig.Load();

        builder.Services.AddDbContext<MiniPDVContext>(options =>
            options.UseSqlServer(dbConfig.ConnectionString));

        builder.Services.AddControllers();
        builder.Services.AddSingleton(sp =>
        {
            var context = sp.GetRequiredService<MiniPDVContext>();
            return new DatabaseInitializer(context, dbConfig);
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

#if WINDOWS
    static void RunWinForms()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            var config = DatabaseConfig.Load();
            var options = new DbContextOptionsBuilder<MiniPDVContext>()
                .UseSqlServer(config.ConnectionString)
                .Options;

            using var context = new MiniPDVContext(options);

            var initializer = new DatabaseInitializer(context, config);

            if (!initializer.IsDatabaseSeeded())
                initializer.Seed();
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
#endif
}
