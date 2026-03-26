using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using storetech_eval_dotnet.Contracts.Cart;
using storetech_eval_dotnet.Data;
using storetech_eval_dotnet.Domain;
using storetech_eval_dotnet.Options;

namespace storetech_eval_dotnet.Services;

public sealed class CartService(
    StoreTechDbContext dbContext,
    IProductService productService,
    ITaxService taxService,
    IOptions<CartOptions> cartOptions) : ICartService
{
    private readonly CartOptions _cartOptions = cartOptions.Value;

    public async Task<IReadOnlyList<CartSummaryDto>> GetCartsAsync(CancellationToken cancellationToken = default)
    {
        var carts = await dbContext.Carts
            .AsNoTracking()
            .Include(cart => cart.Items)
            .OrderByDescending(cart => cart.CreatedAtUtc)
            .ThenBy(cart => cart.Id)
            .ToArrayAsync(cancellationToken);

        return carts
            .Select(CartResponseMapper.ToSummary)
            .ToArray();
    }

    public async Task<int> DeleteCartsAsync(CancellationToken cancellationToken = default)
    {
        var carts = await dbContext.Carts
            .ToArrayAsync(cancellationToken);

        if (carts.Length == 0)
        {
            return 0;
        }

        dbContext.Carts.RemoveRange(carts);
        await dbContext.SaveChangesAsync(cancellationToken);

        return carts.Length;
    }

    public async Task<CartSaveResponse> SaveAsync(
        CartSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Quantity), "Quantity must be greater than zero.");
        }

        var asOfUtc = DateTime.UtcNow;
        var product = await productService.GetProductPricingAsync(
            request.ProductId,
            request.CurrencyCode,
            asOfUtc,
            cancellationToken);
        var taxLookup = await taxService.GetTaxesForRegionAsync(
            request.Region,
            request.CurrencyCode,
            asOfUtc,
            cancellationToken);

        var cart = await GetOrCreateCartAsync(request, taxLookup, asOfUtc, cancellationToken);

        UpsertItem(cart, product, request.Quantity);
        cart.RecalculateTotals(taxLookup.Taxes);

        await dbContext.SaveChangesAsync(cancellationToken);

        var persistedCart = await dbContext.Carts
            .AsNoTracking()
            .Include(candidate => candidate.Items)
            .ThenInclude(item => item.Product)
            .SingleAsync(candidate => candidate.Id == cart.Id, cancellationToken);

        var message = request.CartId.HasValue
            ? "Cart updated successfully."
            : "Cart created successfully.";

        return CartResponseMapper.ToResponse(persistedCart, taxLookup.Taxes, true, message);
    }

    public async Task<int> PurgeStaleCartsAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        var staleCutoffUtc = nowUtc.AddDays(-_cartOptions.StaleCartDays);
        var rewardEligibleCutoffUtc = nowUtc.AddDays(-_cartOptions.RewardEligibleRetentionDays);

        var staleCarts = await dbContext.Carts
            .Where(cart => cart.CreatedAtUtc < staleCutoffUtc)
            .ToListAsync(cancellationToken);

        var cartsToDelete = staleCarts
            .Where(cart => cart.CartType != CartType.RewardEligible || cart.CreatedAtUtc < rewardEligibleCutoffUtc)
            .ToArray();

        if (cartsToDelete.Length == 0)
        {
            return 0;
        }

        dbContext.Carts.RemoveRange(cartsToDelete);
        await dbContext.SaveChangesAsync(cancellationToken);

        return cartsToDelete.Length;
    }

    private async Task<Cart> GetOrCreateCartAsync(
        CartSaveRequest request,
        TaxLookupResult taxLookup,
        DateTime asOfUtc,
        CancellationToken cancellationToken)
    {
        if (!request.CartId.HasValue)
        {
            var newCart = Cart.Create(taxLookup, asOfUtc);
            dbContext.Carts.Add(newCart);
            return newCart;
        }

        var existingCart = await dbContext.Carts
            .Include(candidate => candidate.Items)
            .SingleOrDefaultAsync(candidate => candidate.Id == request.CartId.Value, cancellationToken)
            ?? throw new CartNotFoundException(request.CartId.Value);

        if (!string.Equals(existingCart.Region, taxLookup.Region, StringComparison.Ordinal))
        {
            throw new CartScopeMismatchException(existingCart.Id, "region", existingCart.Region, taxLookup.Region);
        }

        if (!string.Equals(existingCart.CurrencyCode, taxLookup.CurrencyCode, StringComparison.Ordinal))
        {
            throw new CartScopeMismatchException(
                existingCart.Id,
                "currency",
                existingCart.CurrencyCode,
                taxLookup.CurrencyCode);
        }

        return existingCart;
    }

    private void UpsertItem(Cart cart, ProductPricingResult product, int quantity)
    {
        var existingItem = cart.Items.SingleOrDefault(item => item.ProductId == product.ProductId);

        if (existingItem is null)
        {
            var newItem = CartItem.Create(cart, product.ProductId, quantity, product.UnitPrice);    
            
            dbContext.CartItems.Add(newItem);

            return;
        }

        existingItem.AddQuantity(quantity, product.UnitPrice);
    }
}