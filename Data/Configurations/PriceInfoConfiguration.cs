using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using storetech_eval_dotnet.Data.Seed;
using storetech_eval_dotnet.Domain;

namespace storetech_eval_dotnet.Data.Configurations;

public sealed class PriceInfoConfiguration : IEntityTypeConfiguration<PriceInfo>
{
    public void Configure(EntityTypeBuilder<PriceInfo> builder)
    {
        builder.ToTable("PriceInfos");

        builder.HasKey(price => price.Id);

        builder.Property(price => price.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(price => price.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(price => price.EffectiveFromUtc)
            .IsRequired();

        builder.Property(price => price.IsActive)
            .IsRequired();

        builder.HasIndex(price => new { price.ProductId, price.CurrencyCode, price.IsActive });

        builder.HasData(StoreTechSeedData.Prices);
    }
}