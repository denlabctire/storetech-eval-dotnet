using StoreTech.Eval.Tests.TestInfrastructure;

namespace StoreTech.Eval.Tests.Data;

public sealed class CartPersistenceTests
{
    [Fact]
    public async Task PurgeStaleCartsAsync_removes_stale_regular_carts_and_child_items()
    {
        await using var database = new SqliteTestDatabase();
        var staleCartId = Guid.NewGuid();
        var rewardCartId = Guid.NewGuid();
        var freshCartId = Guid.NewGuid();

        await using (var seedContext = database.CreateDbContext())
        {
            seedContext.Carts.AddRange(
                CreateCart(staleCartId, CartType.Regular, DateTime.UtcNow.AddDays(-10), includeItem: true),
                CreateCart(rewardCartId, CartType.RewardEligible, DateTime.UtcNow.AddDays(-10), includeItem: true),
                CreateCart(freshCartId, CartType.Regular, DateTime.UtcNow.AddDays(-2), includeItem: false));

            await seedContext.SaveChangesAsync();
        }

        await using (var purgeContext = database.CreateDbContext())
        {
            var service = new CartService(
                purgeContext,
                new ProductService(purgeContext),
                new TaxService(purgeContext),
                Options.Create(SqliteTestDatabase.CreateCartOptions(staleCartDays: 7, rewardEligibleRetentionDays: 14)));

            var purgedCartCount = await service.PurgeStaleCartsAsync();

            Assert.Equal(1, purgedCartCount);
        }

        await using var assertContext = database.CreateDbContext();
        Assert.False(await assertContext.Carts.AnyAsync(cart => cart.Id == staleCartId));
        Assert.False(await assertContext.CartItems.AnyAsync(item => item.CartId == staleCartId));
        Assert.True(await assertContext.Carts.AnyAsync(cart => cart.Id == rewardCartId));
        Assert.True(await assertContext.Carts.AnyAsync(cart => cart.Id == freshCartId));
    }

    [Fact]
    public async Task PurgeStaleCartsAsync_removes_reward_eligible_carts_after_extended_retention()
    {
        await using var database = new SqliteTestDatabase();
        var rewardCartId = Guid.NewGuid();

        await using (var seedContext = database.CreateDbContext())
        {
            seedContext.Carts.Add(CreateCart(
                rewardCartId,
                CartType.RewardEligible,
                DateTime.UtcNow.AddDays(-20),
                includeItem: false));

            await seedContext.SaveChangesAsync();
        }

        await using (var purgeContext = database.CreateDbContext())
        {
            var service = new CartService(
                purgeContext,
                new ProductService(purgeContext),
                new TaxService(purgeContext),
                Options.Create(SqliteTestDatabase.CreateCartOptions(staleCartDays: 7, rewardEligibleRetentionDays: 14)));

            var purgedCartCount = await service.PurgeStaleCartsAsync();

            Assert.Equal(1, purgedCartCount);
        }

        await using var assertContext = database.CreateDbContext();
        Assert.False(await assertContext.Carts.AnyAsync(cart => cart.Id == rewardCartId));
    }

    private static Cart CreateCart(Guid cartId, CartType cartType, DateTime createdAtUtc, bool includeItem)
    {
        var cart = new Cart
        {
            Id = cartId,
            Region = "CA-AB",
            CurrencyCode = "CAD",
            CartType = cartType,
            CreatedAtUtc = createdAtUtc,
            Subtotal = 18.99m,
            TaxTotal = 0.95m,
            Total = 19.94m
        };

        if (includeItem)
        {
            cart.Items.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 18.99m,
                LineSubtotal = 18.99m
            });
        }

        return cart;
    }
}