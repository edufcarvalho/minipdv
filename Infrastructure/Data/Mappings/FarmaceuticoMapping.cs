using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using minipdv.Domain.Entities;

namespace minipdv.Infrastructure.Data.Mappings;

public class FarmaceuticoMapping : IEntityTypeConfiguration<Farmaceutico>
{
    public void Configure(EntityTypeBuilder<Farmaceutico> builder)
    {
        builder.Property(f => f.Crf)
            .IsRequired()
            .HasMaxLength(12);
    }
}
