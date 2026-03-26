namespace storetech_eval_dotnet.Contracts.Cart;

public sealed class CartSaveResponse
{
    public Guid CartId { get; init; }

    public int TotalItems { get; init; }

    public decimal Subtotal { get; init; }

    public string CartType { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public string CurrencyCode { get; init; } = string.Empty;

    public string Region { get; init; } = string.Empty;

    public IReadOnlyList<CartItemDto> Items { get; init; } = [];

    public IReadOnlyList<TaxBreakdownDto> TaxBreakdown { get; init; } = [];

    public string Message { get; init; } = string.Empty;

    public bool Success { get; init; }
}