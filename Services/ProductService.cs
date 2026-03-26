using Microsoft.EntityFrameworkCore;
using storetech_eval_dotnet.Contracts.Product;
using storetech_eval_dotnet.Data;

namespace storetech_eval_dotnet.Services;

public sealed class ProductService(StoreTechDbContext dbContext) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(
        DateTime asOfUtc,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .Select(product => new ProductDto
            {
                ProductId = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Price = product.Prices
                    .Where(price => price.IsActive
                        && price.EffectiveFromUtc <= asOfUtc
                        && (price.EffectiveToUtc == null || price.EffectiveToUtc > asOfUtc))
                    .OrderByDescending(price => price.EffectiveFromUtc)
                    .Select(price => (decimal?)price.Amount)
                    .FirstOrDefault() ?? 0m,
                CurrencyCode = product.Prices
                    .Where(price => price.IsActive
                        && price.EffectiveFromUtc <= asOfUtc
                        && (price.EffectiveToUtc == null || price.EffectiveToUtc > asOfUtc))
                    .OrderByDescending(price => price.EffectiveFromUtc)
                    .Select(price => price.CurrencyCode)
                    .FirstOrDefault() ?? string.Empty
            })
            .Where(product => product.CurrencyCode != string.Empty)
            .OrderBy(product => product.ProductId)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<ProductPricingResult> GetProductPricingAsync(
        int productId,
        string currencyCode,
        DateTime asOfUtc,
        CancellationToken cancellationToken = default)
    {
        var normalizedCurrency = CartWorkflowNormalization.NormalizeCurrencyCode(currencyCode);

        var product = await dbContext.Products
            .AsNoTracking()
            .Where(candidate => candidate.Id == productId && candidate.IsActive)
            .Select(candidate => new
            {
                candidate.Id,
                candidate.Name,
                candidate.Sku,
                UnitPrice = candidate.Prices
                    .Where(price => price.IsActive
                        && price.CurrencyCode == normalizedCurrency
                        && price.EffectiveFromUtc <= asOfUtc
                        && (price.EffectiveToUtc == null || price.EffectiveToUtc > asOfUtc))
                    .OrderByDescending(price => price.EffectiveFromUtc)
                    .Select(price => (decimal?)price.Amount)
                    .FirstOrDefault()
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(productId);
        }

        if (product.UnitPrice is null)
        {
            throw new UnsupportedCurrencyException(normalizedCurrency,
                $"No active price exists for product '{productId}' in currency '{normalizedCurrency}'.");
        }

        return new ProductPricingResult(
            product.Id,
            product.Name,
            product.Sku,
            normalizedCurrency,
            product.UnitPrice.Value);
    }
}