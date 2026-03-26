using StoreTech.Eval.Tests.TestInfrastructure;

namespace StoreTech.Eval.Tests.Services;

public sealed class CartServiceTests
{
    [Fact]
    public async Task SaveAsync_creates_cart_with_expected_totals()
    {
        await using var database = new SqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();
        var service = CreateCartService(dbContext);

        var response = await service.SaveAsync(new CartSaveRequest
        {
            ProductId = 1,
            Quantity = 2,
            Region = "on",
            CurrencyCode = "cad"
        });

        Assert.True(response.Success);
        Assert.Equal("Cart created successfully.", response.Message);
        Assert.Equal("CA-ON", response.Region);
        Assert.Equal("CAD", response.CurrencyCode);
        Assert.Equal(2, response.TotalItems);
        Assert.Equal(37.98m, response.Subtotal);
        Assert.Single(response.Items);
        Assert.Single(response.TaxBreakdown);
        Assert.Equal(13m, response.TaxBreakdown[0].Percentage);

        var persistedCart = await dbContext.Carts
            .Include(cart => cart.Items)
            .SingleAsync(cart => cart.Id == response.CartId);

        Assert.Equal(37.98m, persistedCart.Subtotal);
        Assert.Equal(4.94m, persistedCart.TaxTotal);
        Assert.Equal(42.92m, persistedCart.Total);
        Assert.Single(persistedCart.Items);
    }

    [Fact]
    public async Task SaveAsync_increments_existing_product_line_quantity()
    {
        await using var database = new SqliteTestDatabase();
        var options = Options.Create(SqliteTestDatabase.CreateCartOptions());
        Guid cartId;

        await using (var createContext = database.CreateDbContext())
        {
            var createService = new CartService(
                createContext,
                new ProductService(createContext),
                new TaxService(createContext),
                options);

            var createdCart = await createService.SaveAsync(new CartSaveRequest
            {
                ProductId = 1,
                Quantity = 1,
                Region = "CA-ON",
                CurrencyCode = "CAD"
            });

            cartId = createdCart.CartId;
        }

        await using (var updateContext = database.CreateDbContext())
        {
            var updateService = new CartService(
                updateContext,
                new ProductService(updateContext),
                new TaxService(updateContext),
                options);

            var updatedCart = await updateService.SaveAsync(new CartSaveRequest
            {
                CartId = cartId,
                ProductId = 1,
                Quantity = 3,
                Region = "ON",
                CurrencyCode = "cad"
            });

            Assert.Equal(cartId, updatedCart.CartId);
            Assert.Equal("Cart updated successfully.", updatedCart.Message);
            Assert.Equal(4, updatedCart.TotalItems);
            Assert.Single(updatedCart.Items);
            Assert.Equal(4, updatedCart.Items[0].Quantity);
        }

        await using var assertContext = database.CreateDbContext();
        var persistedCart = await assertContext.Carts
            .Include(cart => cart.Items)
            .SingleAsync(cart => cart.Id == cartId);

        Assert.Single(persistedCart.Items);
        Assert.Equal(4, persistedCart.Items.Single().Quantity);
        Assert.Equal(75.96m, persistedCart.Subtotal);
        Assert.Equal(9.87m, persistedCart.TaxTotal);
        Assert.Equal(85.83m, persistedCart.Total);
    }

    [Fact]
    public async Task SaveAsync_adds_a_second_distinct_product_to_an_existing_cart()
    {
        await using var database = new SqliteTestDatabase();
        var options = Options.Create(SqliteTestDatabase.CreateCartOptions());
        Guid cartId;

        await using (var createContext = database.CreateDbContext())
        {
            var createService = new CartService(
                createContext,
                new ProductService(createContext),
                new TaxService(createContext),
                options);

            var createdCart = await createService.SaveAsync(new CartSaveRequest
            {
                ProductId = 1,
                Quantity = 2,
                Region = "CA-ON",
                CurrencyCode = "CAD"
            });

            cartId = createdCart.CartId;
        }

        await using (var updateContext = database.CreateDbContext())
        {
            var updateService = new CartService(
                updateContext,
                new ProductService(updateContext),
                new TaxService(updateContext),
                options);

            var updatedCart = await updateService.SaveAsync(new CartSaveRequest
            {
                CartId = cartId,
                ProductId = 2,
                Quantity = 1,
                Region = "ON",
                CurrencyCode = "cad"
            });

            Assert.Equal(cartId, updatedCart.CartId);
            Assert.Equal("Cart updated successfully.", updatedCart.Message);
            Assert.Equal(3, updatedCart.TotalItems);
            Assert.Equal(50.47m, updatedCart.Subtotal);
            Assert.Equal(2, updatedCart.Items.Count);
            Assert.Contains(updatedCart.Items, item => item.ProductId == 1 && item.Quantity == 2);
            Assert.Contains(updatedCart.Items, item => item.ProductId == 2 && item.Quantity == 1);
        }

        await using var assertContext = database.CreateDbContext();
        var persistedCart = await assertContext.Carts
            .Include(cart => cart.Items)
            .SingleAsync(cart => cart.Id == cartId);

        Assert.Equal(2, persistedCart.Items.Count);
        Assert.Equal(50.47m, persistedCart.Subtotal);
        Assert.Equal(6.56m, persistedCart.TaxTotal);
        Assert.Equal(57.03m, persistedCart.Total);
    }

    [Fact]
    public async Task SaveAsync_rejects_region_changes_for_existing_cart()
    {
        await using var database = new SqliteTestDatabase();
        var options = Options.Create(SqliteTestDatabase.CreateCartOptions());
        Guid cartId;

        await using (var createContext = database.CreateDbContext())
        {
            var createService = new CartService(
                createContext,
                new ProductService(createContext),
                new TaxService(createContext),
                options);

            var createdCart = await createService.SaveAsync(new CartSaveRequest
            {
                ProductId = 1,
                Quantity = 1,
                Region = "ON",
                CurrencyCode = "CAD"
            });

            cartId = createdCart.CartId;
        }

        await using var updateContext = database.CreateDbContext();
        var updateService = new CartService(
            updateContext,
            new ProductService(updateContext),
            new TaxService(updateContext),
            options);

        var exception = await Assert.ThrowsAsync<CartScopeMismatchException>(() => updateService.SaveAsync(new CartSaveRequest
        {
            CartId = cartId,
            ProductId = 1,
            Quantity = 1,
            Region = "AB",
            CurrencyCode = "CAD"
        }));

        Assert.Contains("region", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static CartService CreateCartService(StoreTechDbContext dbContext)
    {
        return new CartService(
            dbContext,
            new ProductService(dbContext),
            new TaxService(dbContext),
            Options.Create(SqliteTestDatabase.CreateCartOptions()));
    }
}