using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class VendaMapping : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .ValueGeneratedOnAdd();

        builder.Property(v => v.CriadoEm)
            .IsRequired();

        builder.Property(v => v.AtualizadoEm);

        builder.Property(v => v.CanceladoEm);

        builder.Property(v => v.Total)
            .IsRequired()
            .HasColumnType("decimal(12,4)");

        builder.HasOne(v => v.Vendedor)
            .WithMany()
            .HasForeignKey(v => v.VendedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Cliente)
            .WithMany()
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
