using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class VendaItemMapping : IEntityTypeConfiguration<VendaItem>
{
    public void Configure(EntityTypeBuilder<VendaItem> builder)
    {
        builder.HasKey(vi => new { vi.VendaId, vi.ProdutoId, vi.Posicao });

        builder.Property(vi => vi.Posicao)
            .IsRequired();

        builder.Property(vi => vi.Quantidade)
            .IsRequired()
            .HasDefaultValue(1);

        builder.HasOne(vi => vi.Venda)
            .WithMany(v => v.VendaItens)
            .HasForeignKey(vi => vi.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vi => vi.Produto)
            .WithMany()
            .HasForeignKey(vi => vi.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
