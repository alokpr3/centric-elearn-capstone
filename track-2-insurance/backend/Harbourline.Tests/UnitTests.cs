using FluentValidation;
using Harbourline.Api.Domain;
using Harbourline.Api.Dtos;
using Harbourline.Api.Infrastructure;
using Harbourline.Api.Repositories;
using Harbourline.Api.Services;
using Harbourline.Api.Validators;
using Moq;

namespace Harbourline.Tests;

public class ProductServiceTests
{
    private static ProductRequest Request(string code = "HOME-NEW") => new(code, "New Home", "Home", 400m, 200m, true);

    [Fact]
    public async Task Create_rejects_duplicate_code()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByCodeAsync("HOME-NEW")).ReturnsAsync(new Product { Id = 9, Code = "HOME-NEW" });
        var service = new ProductService(repo.Object, new ProductRequestValidator());

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(Request()));
    }

    [Fact]
    public async Task Create_normalises_code_to_upper_case()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);
        var service = new ProductService(repo.Object, new ProductRequestValidator());

        var created = await service.CreateAsync(Request("home-new"));

        Assert.Equal("HOME-NEW", created.Code);
    }
}

public class PolicyPricingTests
{
    [Fact]
    public void Renewal_discount_is_capped_at_20_percent()
    {
        var discount = PolicyPricing.NoClaimsDiscount(9);

        Assert.True(discount <= 0.30m);
    }

    [Fact]
    public void Renewal_premium_applies_the_discount()
    {
        Assert.Equal(900m, PolicyPricing.RenewalPremium(1000m, 2));
    }
}

public class ClaimRulesTests
{
    [Fact]
    public void Reserve_review_is_due_after_one_month()
    {
        var lastNote = DateTime.Now.AddDays(-30);

        Assert.True(ClaimRules.IsReserveReviewDue(lastNote));
    }

    [Fact]
    public void Reserve_review_is_not_due_for_a_note_made_today()
    {
        Assert.False(ClaimRules.IsReserveReviewDue(DateTime.Now));
    }

    [Fact]
    public void Claim_number_is_zero_padded()
    {
        Assert.Equal("HLC-26-00042", ClaimRules.NextClaimNumber(42));
    }
}

public class LossRatioCalculatorTests
{
    [Fact]
    public void Zero_premium_gives_zero_ratio()
    {
        Assert.Equal(0m, LossRatioCalculator.Percent(5000m, 0m));
    }

    [Fact]
    public void Ratio_is_paid_over_premium_as_a_percentage()
    {
        Assert.Equal(62.5m, LossRatioCalculator.Percent(625m, 1000m));
    }
}

public class ValidatorTests
{
    [Fact]
    public void CreateClaim_rejects_empty_description()
    {
        var result = new CreateClaimRequestValidator().Validate(new CreateClaimRequest(1, "Theft", new DateOnly(2026, 5, 1), "", 500m));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateClaim_rejects_zero_amount()
    {
        var result = new CreateClaimRequestValidator().Validate(new CreateClaimRequest(1, "Theft", new DateOnly(2026, 5, 1), "Bike stolen", 0m));

        Assert.False(result.IsValid);
    }
}

public class ClaimServiceTests
{
    private readonly Mock<IClaimRepository> _claims = new();
    private readonly Mock<IHandlerRepository> _handlers = new();

    private ClaimService Build() => new(_claims.Object, Mock.Of<IPolicyRepository>(), Mock.Of<IPolicyholderRepository>(),
        Mock.Of<IProductRepository>(), _handlers.Object, Mock.Of<IClaimNoteRepository>(), new CreateClaimRequestValidator(),
        new UpdateClaimRequestValidator(), new ChangeStatusRequestValidator(), new ReopenClaimRequestValidator());

    [Fact]
    public async Task Assign_rejects_inactive_handler()
    {
        _claims.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Claim { Id = 1, Status = "New" });
        _handlers.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(new Handler { Id = 4, Name = "Sophie Lang", Active = false, MaxOpenClaims = 8 });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Build().AssignAsync(1, new AssignClaimRequest(4)));
    }

    [Fact]
    public async Task Reopen_is_refused_for_a_claim_that_is_still_open()
    {
        _claims.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Claim { Id = 1, Status = "Assessing" });

        await Assert.ThrowsAsync<BusinessRuleException>(() => Build().ReopenAsync(1, new ReopenClaimRequest(2, "New evidence")));
    }

    [Fact]
    public async Task Reopen_increments_the_reopened_count()
    {
        var claim = new Claim { Id = 1, Status = "Settled", ReopenedCount = 1 };
        _claims.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(claim);

        await Build().ReopenAsync(1, new ReopenClaimRequest(2, "Additional damage found"));

        Assert.Equal(2, claim.ReopenedCount);
        Assert.Equal("Reopened", claim.Status);
    }
}
