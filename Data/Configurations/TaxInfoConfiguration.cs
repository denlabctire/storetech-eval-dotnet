using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using storetech_eval_dotnet.Data.Seed;
using storetech_eval_dotnet.Domain;

namespace storetech_eval_dotnet.Data.Configurations;

public sealed class TaxInfoConfiguration : IEntityTypeConfiguration<TaxInfo>
{
    public void Configure(EntityTypeBuilder<TaxInfo> builder)
    {
        builder.ToTable("TaxInfos");

        builder.HasKey(tax => tax.Id);

        builder.Property(tax => tax.Region)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(tax => tax.Name)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(tax => tax.Rate)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(tax => tax.EffectiveFromUtc)
            .IsRequired();

        builder.Property(tax => tax.IsActive)
            .IsRequired();

        builder.HasIndex(tax => new { tax.Region, tax.IsActive });

        builder.HasData(StoreTechSeedData.Taxes);
    }
}