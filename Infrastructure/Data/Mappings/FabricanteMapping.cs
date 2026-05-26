using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class FabricanteMapping : IEntityTypeConfiguration<Fabricante>
{
    public void Configure(EntityTypeBuilder<Fabricante> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .ValueGeneratedOnAdd();

        builder.Property(f => f.NomeFantasia)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.RazaoSocial)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Cnpj)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(f => f.CriadoEm)
            .IsRequired();

        builder.Property(f => f.AtualizadoEm);

        builder.HasOne(f => f.Contato)
            .WithOne()
            .HasForeignKey<Fabricante>(f => f.ContatoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
