using Microsoft.AspNetCore.Mvc;
using storetech_eval_dotnet.Contracts.Product;
using storetech_eval_dotnet.Services;

namespace storetech_eval_dotnet.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsAsync(DateTime.UtcNow, cancellationToken);
        return Ok(products);
    }
}