using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class VendaProdutoEstoqueMapping : IEntityTypeConfiguration<VendaProdutoEstoque>
{
    public void Configure(EntityTypeBuilder<VendaProdutoEstoque> builder)
    {
        builder.HasKey(vpe => new { vpe.VendaId, vpe.ProdutoId, vpe.Lote });

        builder.Property(vpe => vpe.Quantidade)
            .IsRequired()
            .HasDefaultValue(1);

        builder.HasOne(vpe => vpe.Venda)
            .WithMany(v => v.VendaProdutoEstoques)
            .HasForeignKey(vpe => vpe.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vpe => vpe.ProdutoEstoque)
            .WithMany(pe => pe.VendaProdutoEstoques)
            .HasForeignKey(vpe => new { vpe.ProdutoId, vpe.Lote })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
