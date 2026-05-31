using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class ContatoMapping : IEntityTypeConfiguration<Contato>
{
    public void Configure(EntityTypeBuilder<Contato> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Email)
            .HasMaxLength(200);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        builder.Property(c => c.CriadoEm)
            .IsRequired();

        builder.Property(c => c.AtualizadoEm);
    }
}
