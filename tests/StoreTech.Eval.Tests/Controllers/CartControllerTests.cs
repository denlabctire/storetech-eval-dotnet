using StoreTech.Eval.Tests.TestInfrastructure;

namespace StoreTech.Eval.Tests.Controllers;

public sealed class CartControllerTests
{
    [Fact]
    public async Task Get_api_carts_returns_cart_summaries()
    {
        using var factory = new StoreTechApiFactory();
        using var client = CreateClient(factory);
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreTechDbContext>();

        dbContext.Carts.Add(new Cart
        {
            Id = Guid.NewGuid(),
            Region = "CA-ON",
            CurrencyCode = "CAD",
            CartType = CartType.Regular,
            CreatedAtUtc = DateTime.UtcNow,
            Subtotal = 37.98m,
            TaxTotal = 4.94m,
            Total = 42.92m,
            Items =
            [
                new CartItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = 1,
                    Quantity = 2,
                    UnitPrice = 18.99m,
                    LineSubtotal = 37.98m
                }
            ]
        });
        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync("/api/carts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<IReadOnlyList<CartSummaryDto>>();

        Assert.NotNull(payload);
        Assert.Single(payload);
        Assert.Equal(2, payload[0].TotalItems);
        Assert.Equal(42.92m, payload[0].Total);
        Assert.Equal("CA-ON", payload[0].Region);
    }

    [Fact]
    public async Task Post_api_carts_returns_saved_cart_payload()
    {
        using var factory = new StoreTechApiFactory();
        using var client = CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/carts", new CartSaveRequest
        {
            ProductId = 2,
            Quantity = 2,
            Region = "ab",
            CurrencyCode = "cad"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CartSaveResponse>();

        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.Equal("CA-AB", payload.Region);
        Assert.Equal("CAD", payload.CurrencyCode);
        Assert.Equal("StoreTech Ceramic Mug", payload.Items[0].ProductName);
        Assert.Single(payload.TaxBreakdown);
        Assert.Equal(5m, payload.TaxBreakdown[0].Percentage);
    }

    [Fact]
    public async Task Post_api_carts_returns_validation_problem_for_invalid_request()
    {
        using var factory = new StoreTechApiFactory();
        using var client = CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/carts", new CartSaveRequest
        {
            ProductId = 1,
            Quantity = 0,
            Region = "X",
            CurrencyCode = "CA"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(payload);
        Assert.Contains(nameof(CartSaveRequest.Quantity), payload.Errors.Keys);
        Assert.Contains(nameof(CartSaveRequest.Region), payload.Errors.Keys);
        Assert.Contains(nameof(CartSaveRequest.CurrencyCode), payload.Errors.Keys);
    }

    [Fact]
    public async Task Post_api_carts_returns_not_found_problem_for_missing_cart()
    {
        using var factory = new StoreTechApiFactory();
        using var client = CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/carts", new CartSaveRequest
        {
            CartId = Guid.NewGuid(),
            ProductId = 1,
            Quantity = 1,
            Region = "ON",
            CurrencyCode = "CAD"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(payload);
        Assert.Equal(StatusCodes.Status404NotFound, payload.Status);
        Assert.Equal("Resource not found.", payload.Title);
    }

    [Fact]
    public async Task Delete_api_carts_removes_all_carts()
    {
        using var factory = new StoreTechApiFactory();
        using var client = CreateClient(factory);
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreTechDbContext>();

        dbContext.Carts.AddRange(
            new Cart
            {
                Id = Guid.NewGuid(),
                Region = "CA-ON",
                CurrencyCode = "CAD",
                CartType = CartType.Regular,
                CreatedAtUtc = DateTime.UtcNow,
                Subtotal = 18.99m,
                TaxTotal = 2.47m,
                Total = 21.46m
            },
            new Cart
            {
                Id = Guid.NewGuid(),
                Region = "CA-AB",
                CurrencyCode = "CAD",
                CartType = CartType.Regular,
                CreatedAtUtc = DateTime.UtcNow,
                Subtotal = 12.49m,
                TaxTotal = 0.62m,
                Total = 13.11m
            });
        await dbContext.SaveChangesAsync();

        var response = await client.DeleteAsync("/api/carts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<storetech_eval_dotnet.Controllers.DeleteCartsResponse>();

        Assert.NotNull(payload);
        Assert.Equal(2, payload.DeletedCartCount);
        Assert.False(await dbContext.Carts.AnyAsync());
    }

    [Fact]
    public async Task Delete_api_carts_purge_stale_returns_deleted_count()
    {
        using var factory = new StoreTechApiFactory();
        using var client = CreateClient(factory);
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreTechDbContext>();

        dbContext.Carts.Add(new Cart
        {
            Id = Guid.NewGuid(),
            Region = "CA-ON",
            CurrencyCode = "CAD",
            CartType = CartType.Regular,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-10),
            Subtotal = 18.99m,
            TaxTotal = 2.47m,
            Total = 21.46m
        });
        await dbContext.SaveChangesAsync();

        var response = await client.DeleteAsync("/api/carts/purge-stale");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<storetech_eval_dotnet.Controllers.PurgeStaleCartsResponse>();

        Assert.NotNull(payload);
        Assert.Equal(1, payload.PurgedCartCount);
    }

    private static HttpClient CreateClient(StoreTechApiFactory factory)
    {
        return factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }
}