using Microsoft.AspNetCore.Mvc;
using storetech_eval_dotnet.Contracts.Cart;
using storetech_eval_dotnet.Services;

namespace storetech_eval_dotnet.Controllers;

[ApiController]
[Route("api/carts")]
public sealed class CartController(ICartService cartService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CartSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CartSummaryDto>>> GetCartsAsync(CancellationToken cancellationToken)
    {
        var carts = await cartService.GetCartsAsync(cancellationToken);
        return Ok(carts);
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
        var response = null as CartSaveResponse;
        return Ok(response);
    }

    [HttpDelete]
    [ProducesResponseType<DeleteCartsResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<DeleteCartsResponse>> DeleteCartsAsync(CancellationToken cancellationToken)
    {
        var deletedCartCount = await cartService.DeleteCartsAsync(cancellationToken);
        return Ok(new DeleteCartsResponse(deletedCartCount));
    }

    [HttpDelete("purge-stale")]
    [ProducesResponseType<PurgeStaleCartsResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PurgeStaleCartsResponse>> PurgeStaleCartsAsync(CancellationToken cancellationToken)
    {
        var purgedCartCount = await cartService.PurgeStaleCartsAsync(cancellationToken);
        return Ok(new PurgeStaleCartsResponse(purgedCartCount));
    }
}

public sealed record DeleteCartsResponse(int DeletedCartCount);

public sealed record PurgeStaleCartsResponse(int PurgedCartCount);