using Microsoft.EntityFrameworkCore;
using storetech_eval_dotnet.Data;

namespace storetech_eval_dotnet.Services;

public sealed class TaxService(StoreTechDbContext dbContext) : ITaxService
{
    public async Task<TaxLookupResult> GetTaxesForRegionAsync(
        string region,
        string currencyCode,
        DateTime asOfUtc,
        CancellationToken cancellationToken = default)
    {
        var normalizedCurrency = CartWorkflowNormalization.NormalizeCurrencyCode(currencyCode);

        if (!string.Equals(normalizedCurrency, "CAD", StringComparison.Ordinal))
        {
            throw new UnsupportedCurrencyException(normalizedCurrency);
        }

        var normalizedRegion = CartWorkflowNormalization.NormalizeRegion(region, normalizedCurrency);

        var taxes = await dbContext.Taxes
            .AsNoTracking()
            .Where(tax => tax.IsActive
                && tax.Region == normalizedRegion
                && tax.EffectiveFromUtc <= asOfUtc
                && (tax.EffectiveToUtc == null || tax.EffectiveToUtc > asOfUtc))
            .OrderBy(tax => tax.Name)
            .Select(tax => new TaxRateResult(
                tax.Name,
                decimal.Round(tax.Rate * 100m, 2, MidpointRounding.AwayFromZero),
                tax.Name))
            .ToArrayAsync(cancellationToken);

        if (taxes.Length == 0)
        {
            throw new TaxConfigurationException(normalizedRegion);
        }

        return new TaxLookupResult(normalizedRegion, normalizedCurrency, taxes);
    }
}