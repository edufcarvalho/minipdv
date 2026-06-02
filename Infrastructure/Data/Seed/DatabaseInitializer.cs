using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Infrastructure.Data.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Infrastructure.Configuration;
using minipdv.Infrastructure.Data.Context;
using minipdv.Infrastructure.Security;

namespace minipdv.Infrastructure.Data.Seed;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly MiniPDVContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(MiniPDVContext context, ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public bool IsDatabaseSeeded()
    {
        try
        {
            var exists = _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(1) AS Value FROM sys.tables WHERE name = '__SeedHistory'")
                .AsEnumerable()
                .FirstOrDefault();
            return exists > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Iniciando seed do banco de dados...");
        await SeedAdminUserAsync();
    }

    public async Task SeedDataAsync()
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Iniciando seed de dados...");
                await SeedFuncionariosAsync();

                if (!IsDatabaseSeeded())
                {
                    _logger.LogInformation("Banco não semeado. Executando scripts SQL de seed...");
                    await ExecuteSqlSeedFilesInternalAsync();
                }
                else
                {
                    _logger.LogInformation("Banco já semeado anteriormente. Pulando scripts SQL.");
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Seed de dados concluído com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha durante seed de dados. Realizando rollback...");
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    private async Task ExecuteSqlSeedFilesInternalAsync()
    {
        var sqlFiles = GetSeedFiles();
        if (sqlFiles.Count == 0)
        {
            _logger.LogWarning("Nenhum arquivo SQL de seed encontrado.");
            return;
        }

        _logger.LogInformation("Encontrados {Count} arquivos SQL de seed.", sqlFiles.Count);

        await _context.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID('dbo.__SeedHistory', 'U') IS NULL
            BEGIN
                CREATE TABLE dbo.__SeedHistory (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    ScriptName NVARCHAR(500) NOT NULL,
                    ExecutedAt DATETIME2 NOT NULL DEFAULT GETDATE()
                )
            END");

        foreach (var (filePath, scriptName) in sqlFiles)
        {
            var executed = await _context.Database
                .SqlQueryRaw<int>(
                    "SELECT COUNT(1) AS Value FROM dbo.__SeedHistory WHERE ScriptName = {0}", scriptName)
                .FirstOrDefaultAsync();

            if (executed > 0)
            {
                _logger.LogDebug("Script SQL já executado anteriormente: {ScriptName}", scriptName);
                continue;
            }

            _logger.LogInformation("Executando script SQL: {ScriptName}", scriptName);
            var sql = await File.ReadAllTextAsync(filePath);
            await _context.Database.ExecuteSqlRawAsync(sql);

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO dbo.__SeedHistory (ScriptName) VALUES ({0})", scriptName);

            _logger.LogInformation("Script SQL executado com sucesso: {ScriptName}", scriptName);
        }
    }

    private async Task SeedAdminUserAsync()
    {
        if (await _context.Set<Administrador>().AnyAsync())
        {
            _logger.LogInformation("Administrador já existe. Pulando criação.");
            return;
        }

        var adminPassword = EnvConfig.Get("ADMIN_PASSWORD") ?? throw new InvalidOperationException("ADMIN_PASSWORD environment variable is required for database seeding");

        var admin = new Administrador
        {
            Nome = "Administrador",
            Login = "admin",
            PasswordHash = PasswordHasher.Hash(adminPassword),
            Ativo = true,
            TipoUsuario = "Administrador",
            CriadoEm = DateTime.UtcNow
        };
        _context.Set<Administrador>().Add(admin);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Usuário administrador criado com sucesso. Login={Login}", admin.Login);
    }

    private async Task SeedFuncionariosAsync()
    {
        var now = DateTime.UtcNow;
        var userPassword = EnvConfig.Get("USER_PASSWORD") ?? "mudar123";
        var created = 0;

        if (!await _context.Set<Farmaceutico>().AnyAsync())
        {
            await _context.Set<Farmaceutico>().AddRangeAsync(
                new Farmaceutico
                {
                    Nome = "Felipe Santos",
                    Login = "felipe.farmacia",
                    PasswordHash = PasswordHasher.Hash(userPassword),
                    Ativo = true,
                    TipoUsuario = "Farmaceutico",
                    Crf = "12345-SP",
                    CriadoEm = now
                },
                new Farmaceutico
                {
                    Nome = "Amanda Costa",
                    Login = "amanda.farmacia",
                    PasswordHash = PasswordHasher.Hash(userPassword),
                    Ativo = true,
                    TipoUsuario = "Farmaceutico",
                    Crf = "54321-SP",
                    CriadoEm = now
                }
            );
            created += 2;
            _logger.LogInformation("Farmacêuticos de seed criados: felipe.farmacia, amanda.farmacia");
        }
        else
        {
            _logger.LogInformation("Farmacêuticos já existem. Pulando criação.");
        }

        if (!await _context.Set<Usuario>().AnyAsync())
        {
            await _context.Set<Usuario>().AddRangeAsync(
                new Usuario
                {
                    Nome = "Roberto Alves",
                    Login = "roberto.balcao",
                    PasswordHash = PasswordHasher.Hash(userPassword),
                    Ativo = true,
                    TipoUsuario = "Usuario",
                    CriadoEm = now
                },
                new Usuario
                {
                    Nome = "Camila Nunes",
                    Login = "camila.balcao",
                    PasswordHash = PasswordHasher.Hash(userPassword),
                    Ativo = true,
                    TipoUsuario = "Usuario",
                    CriadoEm = now
                }
            );
            created += 2;
            _logger.LogInformation("Usuários de seed criados: roberto.balcao, camila.balcao");
        }
        else
        {
            _logger.LogInformation("Usuários de balcão já existem. Pulando criação.");
        }

        if (created > 0)
            await _context.SaveChangesAsync();
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

    private List<(string FilePath, string ScriptName)> GetSeedFiles()
    {
        var seedsPath = ResolveSeedsPath();

        if (!Directory.Exists(seedsPath))
            return [];

        return Directory.GetFiles(seedsPath, "*.sql")
            .OrderBy(f => f)
            .Select(f => (f, Path.GetFileName(f)))
            .ToList();
    }
}
