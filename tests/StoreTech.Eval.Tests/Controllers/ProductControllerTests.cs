using StoreTech.Eval.Tests.TestInfrastructure;

namespace StoreTech.Eval.Tests.Controllers;

public sealed class ProductControllerTests
{
    [Fact]
    public async Task Get_api_products_returns_seeded_products()
    {
        using var factory = new StoreTechApiFactory();
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<IReadOnlyList<ProductDto>>();

        Assert.NotNull(payload);
        Assert.Equal(5, payload.Count);
        Assert.Contains(payload, product => product.ProductId == 3 && product.Name == "StoreTech Claw Hammer");
        Assert.Contains(payload, product => product.ProductId == 4 && product.Name == "StoreTech Screwdriver Set");
        Assert.Contains(payload, product => product.ProductId == 5 && product.Name == "StoreTech Hand Saw");
    }
}