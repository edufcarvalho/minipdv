using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Descricao)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Ativo)
            .IsRequired();

        builder.Property(p => p.CriadoEm)
            .IsRequired();

        builder.Property(p => p.AtualizadoEm);

        builder.Property(p => p.CodBarra)
            .IsRequired();

        builder.Property(p => p.Dosagem)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.RegistroMS)
            .HasMaxLength(20);

        builder.Property(p => p.Estoque)
            .IsRequired();

        builder.HasOne(p => p.Grupo)
            .WithMany(g => g.Produtos)
            .HasForeignKey(p => p.ProdutoGrupoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Fabricante)
            .WithMany(f => f.Produtos)
            .HasForeignKey(p => p.FabricanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.PrincipioAtivo)
            .WithMany(pa => pa.Produtos)
            .HasForeignKey(p => p.PrincipioAtivoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasDiscriminator(p => p.Controlado)
            .HasValue<Produto>(false)
            .HasValue<ProdutoControlado>(true);
    }
}
