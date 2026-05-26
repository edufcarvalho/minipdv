using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;
using minipdv.Domain.Entities.Base;

namespace minipdv.Infrastructure.Data.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<AbstractUsuario>
{
    public void Configure(EntityTypeBuilder<AbstractUsuario> builder)
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

        builder.Property(u => u.Ativo)
            .IsRequired();

        builder.Property(u => u.CriadoEm)
            .IsRequired();

        builder.Property(u => u.AtualizadoEm);

        builder.HasOne(u => u.Contato)
            .WithMany()
            .HasForeignKey(u => u.ContatoId);

        builder.HasDiscriminator<string>("TipoUsuario")
            .HasValue<Usuario>("Usuario")
            .HasValue<Farmaceutico>("Farmaceutico");
    }
}
