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
}