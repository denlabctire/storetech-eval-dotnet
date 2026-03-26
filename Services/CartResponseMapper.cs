using storetech_eval_dotnet.Contracts.Cart;
using storetech_eval_dotnet.Domain;

namespace storetech_eval_dotnet.Services;

internal static class CartResponseMapper
{
    public static CartSummaryDto ToSummary(Cart cart)
    {
        return new CartSummaryDto
        {
            CartId = cart.Id,
            TotalItems = cart.Items.Sum(item => item.Quantity),
            Subtotal = cart.Subtotal,
            TaxTotal = cart.TaxTotal,
            Total = cart.Total,
            CartType = cart.CartType.ToString(),
            CreatedAt = cart.CreatedAtUtc,
            CurrencyCode = cart.CurrencyCode,
            Region = cart.Region
        };
    }

    public static CartSaveResponse ToResponse(
        Cart cart,
        IReadOnlyList<TaxRateResult> taxes,
        bool success,
        string message)
    {
        return new CartSaveResponse
        {
            CartId = cart.Id,
            TotalItems = cart.Items.Sum(item => item.Quantity),
            Subtotal = cart.Subtotal,
            CartType = cart.CartType.ToString(),
            CreatedAt = cart.CreatedAtUtc,
            CurrencyCode = cart.CurrencyCode,
            Region = cart.Region,
            Items = cart.Items
                .OrderBy(item => item.ProductId)
                .Select(item => new CartItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Sku = item.Product.Sku,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice,
                    CurrencyCode = cart.CurrencyCode
                })
                .ToArray(),
            TaxBreakdown = taxes
                .Select(tax => new TaxBreakdownDto
                {
                    TaxType = tax.TaxType,
                    Percentage = tax.Percentage,
                    Name = tax.Name
                })
                .ToArray(),
            Message = message,
            Success = success
        };
    }
}