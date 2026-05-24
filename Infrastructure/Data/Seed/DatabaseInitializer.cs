using Microsoft.EntityFrameworkCore;
using minipdv.Application.Interfaces;
using minipdv.Infrastructure.Configuration;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Seed;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly MiniPDVContext _context;
    private readonly string _seedsPath;

    public DatabaseInitializer(MiniPDVContext context, string seedsPath)
    {
        _context = context;
        _seedsPath = seedsPath;
    }

    public bool IsDatabaseSeeded()
    {
        try
        {
            var exists = _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(1) FROM sys.tables WHERE name = '__SeedHistory'")
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
        if (IsDatabaseSeeded())
            return;

        var sqlFiles = GetSqlFiles();
        if (sqlFiles.Count == 0)
            return;

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
                        "SELECT COUNT(1) FROM dbo.__SeedHistory WHERE ScriptName = {0}", scriptName)
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
    }

    public async Task SeedAsync()
    {
        if (IsDatabaseSeeded())
            return;

        var sqlFiles = GetSqlFiles();
        if (sqlFiles.Count == 0)
            return;

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
                        "SELECT COUNT(1) FROM dbo.__SeedHistory WHERE ScriptName = {0}", scriptName)
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
    }

    private List<(string FilePath, string ScriptName)> GetSqlFiles()
    {
        if (!Directory.Exists(_seedsPath))
            return [];

        return Directory.GetFiles(_seedsPath, "*.sql")
            .OrderBy(f => f)
            .Select(f => (f, Path.GetFileName(f)))
            .ToList();
    }
}
