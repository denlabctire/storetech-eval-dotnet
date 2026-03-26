namespace storetech_eval_dotnet.Services;

public sealed record TaxRateResult(
    string TaxType,
    decimal Percentage,
    string Name);

public sealed record TaxLookupResult(
    string Region,
    string CurrencyCode,
    IReadOnlyList<TaxRateResult> Taxes);