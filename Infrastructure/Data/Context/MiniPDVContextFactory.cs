using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using minipdv.Infrastructure.Configuration;

namespace minipdv.Infrastructure.Data.Context;

public class MiniPDVContextFactory : IDesignTimeDbContextFactory<MiniPDVContext>
{
    public MiniPDVContext CreateDbContext(string[] args)
    {
        var config = DatabaseConfig.Load();

        var optionsBuilder = new DbContextOptionsBuilder<MiniPDVContext>();
        optionsBuilder.UseSqlServer(config.ConnectionString);

        return new MiniPDVContext(optionsBuilder.Options);
    }
}
