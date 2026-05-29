using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Context;

public class MiniPDVContext : DbContext
{

    public MiniPDVContext(DbContextOptions<MiniPDVContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Farmaceutico> Farmaceuticos { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<ProdutoControlado> ProdutosControlados { get; set; }
    public DbSet<ProdutoGrupo> ProdutoGrupos { get; set; }
    public DbSet<ProdutoTipo> ProdutoTipos { get; set; }
    public DbSet<Fabricante> Fabricantes { get; set; }
    public DbSet<Contato> Contatos { get; set; }
    public DbSet<PrincipioAtivo> PrincipiosAtivos { get; set; }
    public DbSet<ProdutoEstoque> ProdutoEstoques { get; set; }
    public DbSet<ProdutoCodBarra> ProdutoCodBarras { get; set; }
    public DbSet<Cliente> Clientes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiniPDVContext).Assembly);
    }
}
