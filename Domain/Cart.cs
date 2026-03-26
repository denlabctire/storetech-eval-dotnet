namespace storetech_eval_dotnet.Domain;

public sealed class Cart
{
    public Guid Id { get; set; }

    public string Region { get; set; } = string.Empty;

    public string CurrencyCode { get; set; } = string.Empty;

    public CartType CartType { get; set; } = CartType.Regular;

    public DateTime CreatedAtUtc { get; set; }

    public decimal Subtotal { get; set; }

    public decimal TaxTotal { get; set; }

    public decimal Total { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}