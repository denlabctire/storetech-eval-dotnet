namespace storetech_eval_dotnet.Services;

internal static class CartWorkflowNormalization
{
    public static string NormalizeCurrencyCode(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new ArgumentException("Currency code is required.", nameof(currencyCode));
        }

        var normalizedCurrency = currencyCode.Trim().ToUpperInvariant();

        if (normalizedCurrency.Length != 3 || normalizedCurrency.Any(character => !char.IsLetter(character)))
        {
            throw new ArgumentException("Currency code must be a three-letter ISO code.", nameof(currencyCode));
        }

        return normalizedCurrency;
    }

    public static string NormalizeRegion(string region, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(region))
        {
            throw new ArgumentException("Region is required.", nameof(region));
        }

        var normalizedRegion = region.Trim().ToUpperInvariant();

        if (normalizedRegion.Length == 2 && string.Equals(currencyCode, "CAD", StringComparison.Ordinal))
        {
            return $"CA-{normalizedRegion}";
        }

        if (normalizedRegion.Length == 5 && normalizedRegion[2] == '-')
        {
            return normalizedRegion;
        }

        throw new ArgumentException("Region must use a province code such as ON or CA-ON.", nameof(region));
    }
}