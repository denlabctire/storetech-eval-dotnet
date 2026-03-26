using storetech_eval_dotnet.Contracts.Cart;

namespace storetech_eval_dotnet.Services;

public interface ICartService
{
	Task<IReadOnlyList<CartSummaryDto>> GetCartsAsync(CancellationToken cancellationToken = default);

	Task<int> DeleteCartsAsync(CancellationToken cancellationToken = default);

	Task<CartSaveResponse> SaveAsync(CartSaveRequest request, CancellationToken cancellationToken = default);

	Task<int> PurgeStaleCartsAsync(CancellationToken cancellationToken = default);
}