namespace storetech_eval_dotnet.Contracts.Cart;

public sealed class CartSummaryDto
{
    public Guid CartId { get; init; }

    public int TotalItems { get; init; }

    public decimal Subtotal { get; init; }

    public decimal TaxTotal { get; init; }

    public decimal Total { get; init; }

    public string CartType { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public string CurrencyCode { get; init; } = string.Empty;

    public string Region { get; init; } = string.Empty;
}