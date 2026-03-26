using storetech_eval_dotnet.Domain;

namespace storetech_eval_dotnet.Data.Seed;

internal static class StoreTechSeedData
{
    private static readonly DateTime SeedEffectiveFromUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Product[] Products =
    [
        new Product
        {
            Id = 1,
            Sku = "SKU-COFFEE-001",
            Name = "StoreTech Coffee Beans",
            IsActive = true
        },
        new Product
        {
            Id = 2,
            Sku = "SKU-MUG-001",
            Name = "StoreTech Ceramic Mug",
            IsActive = true
        },
        new Product
        {
            Id = 3,
            Sku = "SKU-HAMMER-001",
            Name = "StoreTech Claw Hammer",
            IsActive = true
        },
        new Product
        {
            Id = 4,
            Sku = "SKU-SCREWDRIVER-001",
            Name = "StoreTech Screwdriver Set",
            IsActive = true
        },
        new Product
        {
            Id = 5,
            Sku = "SKU-SAW-001",
            Name = "StoreTech Hand Saw",
            IsActive = true
        }
    ];

    public static readonly PriceInfo[] Prices =
    [
        new PriceInfo
        {
            Id = 1,
            ProductId = 1,
            CurrencyCode = "CAD",
            Amount = 18.99m,
            EffectiveFromUtc = SeedEffectiveFromUtc,
            IsActive = true
        },
        new PriceInfo
        {
            Id = 2,
            ProductId = 2,
            CurrencyCode = "CAD",
            Amount = 12.49m,
            EffectiveFromUtc = SeedEffectiveFromUtc,
            IsActive = true
        },
        new PriceInfo
        {
            Id = 3,
            ProductId = 3,
            CurrencyCode = "CAD",
            Amount = 24.99m,
            EffectiveFromUtc = SeedEffectiveFromUtc,
            IsActive = true
        },
        new PriceInfo
        {
            Id = 4,
            ProductId = 4,
            CurrencyCode = "CAD",
            Amount = 19.99m,
            EffectiveFromUtc = SeedEffectiveFromUtc,
            IsActive = true
        },
        new PriceInfo
        {
            Id = 5,
            ProductId = 5,
            CurrencyCode = "CAD",
            Amount = 29.99m,
            EffectiveFromUtc = SeedEffectiveFromUtc,
            IsActive = true
        }
    ];

    public static readonly TaxInfo[] Taxes =
    [
        new TaxInfo
        {
            Id = 1,
            Region = "CA-ON",
            Name = "HST",
            Rate = 0.13m,
            EffectiveFromUtc = SeedEffectiveFromUtc,
            IsActive = true
        },
        new TaxInfo
        {
            Id = 2,
            Region = "CA-AB",
            Name = "GST",
            Rate = 0.05m,
            EffectiveFromUtc = SeedEffectiveFromUtc,
            IsActive = true
        }
    ];
}