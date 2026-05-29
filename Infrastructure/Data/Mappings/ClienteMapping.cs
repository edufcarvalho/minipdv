using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class ClienteMapping : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Cpf)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(c => c.CriadoEm)
            .IsRequired();

        builder.Property(c => c.AtualizadoEm);

        builder.HasOne(c => c.Contato)
            .WithOne()
            .HasForeignKey<Cliente>(c => c.ContatoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
