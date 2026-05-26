using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class ProdutoGrupoMapping : IEntityTypeConfiguration<ProdutoGrupo>
{
    public void Configure(EntityTypeBuilder<ProdutoGrupo> builder)
    {
        builder.HasKey(pg => pg.Id);

        builder.Property(pg => pg.Id)
            .ValueGeneratedOnAdd();

        builder.Property(pg => pg.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pg => pg.CriadoEm)
            .IsRequired();
    }
}
