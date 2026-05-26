using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class ProdutoEstoqueMapping : IEntityTypeConfiguration<ProdutoEstoque>
{
    public void Configure(EntityTypeBuilder<ProdutoEstoque> builder)
    {
        builder.HasKey(p => new { p.ProdutoId, p.Lote });

        builder.Property(p => p.Lote)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Fabricacao)
            .HasColumnType("date");

        builder.Property(p => p.Validade)
            .HasColumnType("date");

        builder.Property(p => p.Quantidade)
            .IsRequired();

        builder.HasOne(p => p.Produto)
            .WithMany()
            .HasForeignKey(p => p.ProdutoId);
    }
}
