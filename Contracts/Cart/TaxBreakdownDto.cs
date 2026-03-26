namespace storetech_eval_dotnet.Contracts.Cart;

public sealed class TaxBreakdownDto
{
    public string TaxType { get; init; } = string.Empty;

    public decimal Percentage { get; init; }

    public string Name { get; init; } = string.Empty;
}