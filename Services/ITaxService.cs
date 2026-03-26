namespace storetech_eval_dotnet.Services;

public interface ITaxService
{
	Task<TaxLookupResult> GetTaxesForRegionAsync(
		string region,
		string currencyCode,
		DateTime asOfUtc,
		CancellationToken cancellationToken = default);
}