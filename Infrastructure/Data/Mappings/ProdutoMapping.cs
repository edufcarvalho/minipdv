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
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(p => p.Dosagem)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(p => p.Grupo)
            .WithMany(g => g.Produtos)
            .HasForeignKey(p => p.ProdutoGrupoId);

        builder.HasOne(p => p.Fabricante)
            .WithMany(f => f.Produtos)
            .HasForeignKey(p => p.FabricanteId);

        builder.HasOne(p => p.PrincipioAtivo)
            .WithMany(pa => pa.Produtos)
            .HasForeignKey(p => p.PrincipioAtivoId);

        builder.HasDiscriminator(p => p.Controlado)
            .HasValue<Produto>(false)
            .HasValue<ProdutoControlado>(true);
    }
}
