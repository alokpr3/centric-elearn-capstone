namespace Harbourline.Api.Services;

public static class PolicyPricing
{
    public static decimal NoClaimsDiscount(int noClaimsYears) => Math.Min(noClaimsYears * 0.05m, 0.30m);

    public static decimal RenewalPremium(decimal currentPremium, int noClaimsYears) =>
        Math.Round(currentPremium * (1 - NoClaimsDiscount(noClaimsYears)), 2);
}

public static class ClaimRules
{
    public static bool IsReserveReviewDue(DateTime lastNoteAt) => DateTime.Now.Date >= lastNoteAt.Date.AddMonths(1);

    public static string NextClaimNumber(int sequence) => $"HLC-26-{sequence:D5}";
}

public static class LossRatioCalculator
{
    public static decimal Percent(decimal paid, decimal premium) => premium == 0 ? 0 : Math.Round(paid / premium * 100m, 1);
}
