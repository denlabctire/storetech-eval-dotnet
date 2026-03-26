namespace storetech_eval_dotnet.Domain;

public sealed class CartItem
{
    public Guid Id { get; set; }

    public Guid CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineSubtotal { get; set; }

    public Cart Cart { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public static CartItem Create(Cart cart, int productId, int quantity, decimal unitPrice) => new()
    {
        Id = Guid.NewGuid(),
        ProductId = productId,
        Quantity = quantity,
        Cart = cart,
        UnitPrice = unitPrice,
        LineSubtotal = quantity * unitPrice
    };

    public void AddQuantity(int additionalQuantity, decimal unitPrice)
    {
        UnitPrice = unitPrice;
        Quantity += additionalQuantity;
        LineSubtotal = Quantity * UnitPrice;
    }
}