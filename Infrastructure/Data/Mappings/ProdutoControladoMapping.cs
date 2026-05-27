using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class ProdutoControladoMapping : IEntityTypeConfiguration<ProdutoControlado>
{
    public void Configure(EntityTypeBuilder<ProdutoControlado> builder)
    {
        builder.Property(p => p.RegistroMS)
            .HasMaxLength(20);

        builder.Property(p => p.ClasseTerapeutica)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Lista)
            .IsRequired()
            .HasMaxLength(50);
    }
}
