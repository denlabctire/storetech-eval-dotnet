using System.ComponentModel.DataAnnotations;

namespace storetech_eval_dotnet.Contracts.Cart;

public sealed class CartSaveRequest
{
    public Guid? CartId { get; init; }

    [Range(1, int.MaxValue)]
    public int ProductId { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    [Required]
    [StringLength(32, MinimumLength = 2)]
    [RegularExpression("^[A-Za-z]{2}(?:-[A-Za-z]{2})?$")]
    public string Region { get; init; } = string.Empty;

    [Required]
    [StringLength(3, MinimumLength = 3)]
    [RegularExpression("^[A-Za-z]{3}$")]
    public string CurrencyCode { get; init; } = string.Empty;
}