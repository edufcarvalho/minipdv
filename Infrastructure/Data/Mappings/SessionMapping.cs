using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class SessionMapping : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Token)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(s => s.DeviceInfo)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(s => s.ExpiresAt)
            .IsRequired();

        builder.Property(s => s.IsRevoked)
            .IsRequired();

        builder.Property(s => s.CriadoEm)
            .IsRequired();

        builder.Property(s => s.AtualizadoEm);

        builder.HasOne(s => s.Usuario)
            .WithMany()
            .HasForeignKey(s => s.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.Token)
            .IsUnique();
    }
}
