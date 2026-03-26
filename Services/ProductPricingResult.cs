namespace storetech_eval_dotnet.Services;

public sealed record ProductPricingResult(
    int ProductId,
    string ProductName,
    string Sku,
    string CurrencyCode,
    decimal UnitPrice);