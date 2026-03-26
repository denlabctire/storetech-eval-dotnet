namespace storetech_eval_dotnet.Domain;

public sealed class PriceInfo
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime EffectiveFromUtc { get; set; }

    public DateTime? EffectiveToUtc { get; set; }

    public bool IsActive { get; set; }

    public Product Product { get; set; } = null!;
}