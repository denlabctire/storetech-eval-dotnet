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

    public static Cart Create(storetech_eval_dotnet.Services.TaxLookupResult taxLookup, DateTime asOfUtc)
    {
        return new Cart
        {
            Id = Guid.NewGuid(),
            Region = taxLookup.Region,
            CurrencyCode = taxLookup.CurrencyCode,
            CreatedAtUtc = asOfUtc
        };
    }

    public void RecalculateTotals(IReadOnlyList<storetech_eval_dotnet.Services.TaxRateResult> taxes)
    {
        Subtotal = decimal.Round(
            Items.Sum(item => item.LineSubtotal),
            2,
            MidpointRounding.AwayFromZero);

        TaxTotal = decimal.Round(
            taxes.Sum(tax => Subtotal * (tax.Percentage / 100m)),
            2,
            MidpointRounding.AwayFromZero);

        Total = decimal.Round(
            Subtotal + TaxTotal,
            2,
            MidpointRounding.AwayFromZero);
    }
}