namespace storetech_eval_dotnet.Domain;

public sealed class Product
{
    public int Id { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public ICollection<PriceInfo> Prices { get; set; } = new List<PriceInfo>();

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}