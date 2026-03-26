using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using storetech_eval_dotnet.Domain;

namespace storetech_eval_dotnet.Data.Configurations;

public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(cart => cart.Id);

        builder.Property(cart => cart.Region)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(cart => cart.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(cart => cart.CartType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(cart => cart.CreatedAtUtc)
            .IsRequired();

        builder.Property(cart => cart.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cart => cart.TaxTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(cart => cart.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasMany(cart => cart.Items)
            .WithOne(item => item.Cart)
            .HasForeignKey(item => item.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cart => cart.CreatedAtUtc);
        builder.HasIndex(cart => new { cart.Region, cart.CurrencyCode });
    }
}