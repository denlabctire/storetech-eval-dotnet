using Microsoft.AspNetCore.Mvc;

namespace storetech_eval_dotnet.Controllers;

[ApiController]
[Route("")]
public sealed class HomeController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<StoreTechApiInfoResponse>(StatusCodes.Status200OK)]
    public ActionResult<StoreTechApiInfoResponse> GetApiInfo()
    {
        return Ok(new StoreTechApiInfoResponse(
            "StoreTech cart API",
            "This host exposes the cart-first migration slice with product catalog and cart management endpoints.",
            [
                new StoreTechEndpointInfo("GET", "/api/products", "Returns the seeded product catalog with active pricing."),
                new StoreTechEndpointInfo("GET", "/api/carts", "Returns the persisted carts in the current database."),
                new StoreTechEndpointInfo("POST", "/api/carts", "Creates or updates a cart using the seeded product catalog and tax rules."),
                new StoreTechEndpointInfo("DELETE", "/api/carts", "Deletes all persisted carts from the current database."),
                new StoreTechEndpointInfo("DELETE", "/api/carts/purge-stale", "Removes stale carts based on the configured retention policy.")
            ]));
    }
}

public sealed record StoreTechApiInfoResponse(
    string Service,
    string Description,
    IReadOnlyList<StoreTechEndpointInfo> Endpoints);

public sealed record StoreTechEndpointInfo(
    string Method,
    string Path,
    string Description);
