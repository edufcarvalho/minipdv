using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class ReceitaProdutoEstoqueMapping : IEntityTypeConfiguration<ReceitaProdutoEstoque>
{
    public void Configure(EntityTypeBuilder<ReceitaProdutoEstoque> builder)
    {
        builder.HasKey(rpe => new { rpe.ReceitaId, rpe.ProdutoId, rpe.Lote });

        builder.Property(rpe => rpe.Quantidade)
            .IsRequired()
            .HasDefaultValue(1);

        builder.HasOne(rpe => rpe.Receita)
            .WithMany(r => r.ReceitaProdutoEstoques)
            .HasForeignKey(rpe => rpe.ReceitaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rpe => rpe.ProdutoEstoque)
            .WithMany(pe => pe.ReceitaProdutoEstoques)
            .HasForeignKey(rpe => new { rpe.ProdutoId, rpe.Lote })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
