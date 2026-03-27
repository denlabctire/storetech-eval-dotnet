using Microsoft.AspNetCore.Mvc;
using storetech_eval_dotnet.Contracts.Cart;

namespace storetech_eval_dotnet.Controllers;

[ApiController]
[Route("api/carts")]
public sealed class CartController() : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CartSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CartSummaryDto>>> GetCartsAsync(CancellationToken cancellationToken)
    {
       throw new NotImplementedException("This method is not implemented yet.");
    }

    [HttpPost]
    [ProducesResponseType<CartSaveResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartSaveResponse>> SaveCartAsync(
        [FromBody] CartSaveRequest request,
        CancellationToken cancellationToken)
    {
        // TODO implement this method so that it validates the request, 
        // saves the cart and returns the appropriate response.
        throw new NotImplementedException("This method is not implemented yet.");
    }

    [HttpDelete]
    [ProducesResponseType<DeleteCartsResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<DeleteCartsResponse>> DeleteCartsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    [HttpDelete("purge-stale")]
    [ProducesResponseType<PurgeStaleCartsResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PurgeStaleCartsResponse>> PurgeStaleCartsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }
}

public sealed record DeleteCartsResponse(int DeletedCartCount);

public sealed record PurgeStaleCartsResponse(int PurgedCartCount);