using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class ReceitaMapping : IEntityTypeConfiguration<Receita>
{
    public void Configure(EntityTypeBuilder<Receita> builder)
    {
        builder.ToTable("Receitas");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.DataReceita)
            .IsRequired();

        builder.Property(r => r.DataCadastro)
            .IsRequired();

        builder.Property(r => r.CriadoEm)
            .IsRequired();

        builder.Property(r => r.AtualizadoEm);

        builder.HasOne(r => r.Prescritor)
            .WithMany()
            .HasForeignKey(r => r.PrescritorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Paciente)
            .WithMany()
            .HasForeignKey(r => r.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Comprador)
            .WithMany()
            .HasForeignKey(r => r.CompradorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
