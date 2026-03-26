namespace storetech_eval_dotnet.Domain;

public sealed class TaxInfo
{
    public int Id { get; set; }

    public string Region { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Rate { get; set; }

    public DateTime EffectiveFromUtc { get; set; }

    public DateTime? EffectiveToUtc { get; set; }

    public bool IsActive { get; set; }
}