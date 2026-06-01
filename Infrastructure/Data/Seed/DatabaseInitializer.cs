using Microsoft.EntityFrameworkCore;
using minipdv.Infrastructure.Data.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Infrastructure.Configuration;
using minipdv.Infrastructure.Data.Context;
using minipdv.Infrastructure.Security;

namespace minipdv.Infrastructure.Data.Seed;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly MiniPDVContext _context;

    public DatabaseInitializer(MiniPDVContext context)
    {
        _context = context;
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

    public void Seed()
    {
        SeedAdminUser();
    }

    public async Task SeedAsync()
    {
        await SeedAdminUserAsync();
    }

    public void SeedData()
    {
        SeedFuncionarios();

        if (!IsDatabaseSeeded())
            ExecuteSqlSeedFiles();
    }

    public async Task SeedDataAsync()
    {
        await SeedFuncionariosAsync();

        if (!IsDatabaseSeeded())
            await ExecuteSqlSeedFilesAsync();
    }

    private void ExecuteSqlSeedFiles()
    {
        var sqlFiles = GetSqlFiles();
        if (sqlFiles.Count == 0)
            return;

        _context.Database.CreateExecutionStrategy().Execute(() =>
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Database.ExecuteSqlRaw(@"
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
                    var alreadyExecuted = _context.Database
                        .SqlQueryRaw<int>(
                            "SELECT COUNT(1) AS Value FROM dbo.__SeedHistory WHERE ScriptName = {0}", scriptName)
                        .AsEnumerable()
                        .FirstOrDefault() > 0;

                    if (alreadyExecuted)
                        continue;

                    var sql = File.ReadAllText(filePath);
                    _context.Database.ExecuteSqlRaw(sql);

                    _context.Database.ExecuteSqlRaw(
                        "INSERT INTO dbo.__SeedHistory (ScriptName) VALUES ({0})", scriptName);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });
    }

    private async Task ExecuteSqlSeedFilesAsync()
    {
        var sqlFiles = GetSqlFiles();
        if (sqlFiles.Count == 0)
            return;

        await _context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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
                        continue;

                    var sql = await File.ReadAllTextAsync(filePath);
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO dbo.__SeedHistory (ScriptName) VALUES ({0})", scriptName);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    private void SeedAdminUser()
    {
        if (_context.Set<Administrador>().Any()) return;

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
        _context.SaveChanges();
    }

    private async Task SeedAdminUserAsync()
    {
        if (await _context.Set<Administrador>().AnyAsync()) return;

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
    }

    private void SeedFuncionarios()
    {
        var now = DateTime.UtcNow;
        var userPassword = EnvConfig.Get("USER_PASSWORD") ?? "mudar123";

        if (!_context.Set<Farmaceutico>().Any())
        {
            _context.Set<Farmaceutico>().AddRange(
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
        }

        if (!_context.Set<Usuario>().Any())
        {
            _context.Set<Usuario>().AddRange(
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
        }

        _context.SaveChanges();
    }

    private async Task SeedFuncionariosAsync()
    {
        var now = DateTime.UtcNow;
        var userPassword = EnvConfig.Get("USER_PASSWORD") ?? "mudar123";

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
        }

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

    private List<(string FilePath, string ScriptName)> GetSqlFiles()
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
