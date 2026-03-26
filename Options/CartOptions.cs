using System.ComponentModel.DataAnnotations;

namespace storetech_eval_dotnet.Options;

public sealed class CartOptions
{
    public const string SectionName = "Cart";

    [Range(1, int.MaxValue)]
    public int StaleCartDays { get; init; }

    [Range(1, int.MaxValue)]
    public int RewardEligibleRetentionDays { get; init; }
}