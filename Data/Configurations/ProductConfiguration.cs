using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using storetech_eval_dotnet.Data.Seed;
using storetech_eval_dotnet.Domain;

namespace storetech_eval_dotnet.Data.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Sku)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(product => product.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(product => product.IsActive)
            .IsRequired();

        builder.HasIndex(product => product.Sku)
            .IsUnique();

        builder.HasMany(product => product.Prices)
            .WithOne(price => price.Product)
            .HasForeignKey(price => price.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(StoreTechSeedData.Products);
    }
}