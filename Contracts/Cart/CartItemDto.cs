namespace storetech_eval_dotnet.Contracts.Cart;

public sealed class CartItemDto
{
    public int ProductId { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public string Sku { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public decimal Price { get; init; }

    public string CurrencyCode { get; init; } = string.Empty;
}