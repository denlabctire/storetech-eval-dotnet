using storetech_eval_dotnet.Contracts.Product;

namespace storetech_eval_dotnet.Services;

public interface IProductService
{
	Task<IReadOnlyList<ProductDto>> GetProductsAsync(
		DateTime asOfUtc,
		CancellationToken cancellationToken = default);

	Task<ProductPricingResult> GetProductPricingAsync(
		int productId,
		string currencyCode,
		DateTime asOfUtc,
		CancellationToken cancellationToken = default);
}