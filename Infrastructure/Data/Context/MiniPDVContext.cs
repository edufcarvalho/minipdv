using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Context;

public class MiniPDVContext : DbContext
{

    public MiniPDVContext(DbContextOptions<MiniPDVContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiniPDVContext).Assembly);
    }
}
