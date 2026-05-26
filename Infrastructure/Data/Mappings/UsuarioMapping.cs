using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<UsuarioBase>
{
    public void Configure(EntityTypeBuilder<UsuarioBase> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Login)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasDiscriminator<string>("TipoUsuario")
            .HasValue<Usuario>("Usuario")
            .HasValue<Farmaceutico>("Farmaceutico");
    }
}
