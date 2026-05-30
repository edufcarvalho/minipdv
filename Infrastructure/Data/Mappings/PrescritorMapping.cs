using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class PrescritorMapping : IEntityTypeConfiguration<Prescritor>
{
    public void Configure(EntityTypeBuilder<Prescritor> builder)
    {
        builder.ToTable("Prescritores");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Numero)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Conselho)
            .IsRequired()
            .HasMaxLength(7)
            .HasConversion<string>();

        builder.Property(p => p.Uf)
            .IsRequired()
            .HasMaxLength(2)
            .HasConversion<string>();

        builder.Property(p => p.CriadoEm)
            .IsRequired();

        builder.Property(p => p.AtualizadoEm);

        builder.HasIndex(p => new { p.Numero, p.Conselho, p.Uf })
            .IsUnique();
    }
}
