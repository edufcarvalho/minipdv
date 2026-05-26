using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class ProdutoCodBarraMapping : IEntityTypeConfiguration<ProdutoCodBarra>
{
    public void Configure(EntityTypeBuilder<ProdutoCodBarra> builder)
    {
        builder.HasKey(p => p.CodBarra);

        builder.Property(p => p.CodBarra)
            .ValueGeneratedNever();

        builder.Property(p => p.CriadoEm)
            .IsRequired();

        builder.HasOne(p => p.Produto)
            .WithMany(p => p.CodBarras)
            .HasForeignKey(p => p.ProdutoId);
    }
}
