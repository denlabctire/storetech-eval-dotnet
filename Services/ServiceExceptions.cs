namespace storetech_eval_dotnet.Services;

public sealed class CartNotFoundException(Guid cartId)
    : KeyNotFoundException($"Cart '{cartId}' was not found.")
{
}

public sealed class CartScopeMismatchException(Guid cartId, string scopeName, string existingValue, string requestedValue)
    : InvalidOperationException(
        $"Cart '{cartId}' is scoped to {scopeName} '{existingValue}' and cannot be updated for {scopeName} '{requestedValue}'.")
{
}

public sealed class ProductNotFoundException(int productId)
    : KeyNotFoundException($"Product '{productId}' was not found.")
{
}

public sealed class UnsupportedCurrencyException(string currencyCode, string? reason = null)
    : InvalidOperationException(reason ?? $"Currency '{currencyCode}' is not supported for the cart workflow.")
{
}

public sealed class TaxConfigurationException(string region)
    : InvalidOperationException($"No active taxes are configured for region '{region}'.")
{
}