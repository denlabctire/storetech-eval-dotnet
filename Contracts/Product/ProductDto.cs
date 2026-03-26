namespace storetech_eval_dotnet.Contracts.Product;

public sealed class ProductDto
{
    public int ProductId { get; init; }

    public string Sku { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string CurrencyCode { get; init; } = string.Empty;
}