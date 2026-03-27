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
        throw new NotImplementedException("This method is not implemented yet.");
    }

    public async Task<int> DeleteCartsAsync(CancellationToken cancellationToken = default)
    {
       throw new NotImplementedException("This method is not implemented yet.");
    }

    /**
        * This method should:
        * - Validate the request (e.g. check that the products exist, that the quantities are positive, etc.)
        * - Create or update the cart and cart items in the database
        * - Recalculate the cart totals using the tax service
        * - Return a CartSaveResponse with the appropriate data
        *
        * see the challenge-001-intermediate-cart-impl.md file for more details and hints on how to implement this method.
        */
    public async Task<CartSaveResponse> SaveAsync(
        CartSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        // TODO implement this method so that it validates the request,
        // saves the cart and returns the appropriate dto.
        throw new NotImplementedException("This method is not implemented yet.");
    }

    public async Task<int> PurgeStaleCartsAsync(CancellationToken cancellationToken = default)
    {
       throw new NotImplementedException("This method is not implemented yet.");
    }

}