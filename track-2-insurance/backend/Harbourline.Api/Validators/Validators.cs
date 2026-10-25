using FluentValidation;
using Harbourline.Api.Domain;
using Harbourline.Api.Dtos;

namespace Harbourline.Api.Validators;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Line).Must(l => l is "Home" or "Motor").WithMessage("Line must be Home or Motor.");
        RuleFor(x => x.BasePremium).GreaterThan(0);
        RuleFor(x => x.DefaultExcess).GreaterThanOrEqualTo(0);
    }
}

public class HandlerRequestValidator : AbstractValidator<HandlerRequest>
{
    public HandlerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.MaxOpenClaims).InclusiveBetween(1, 100);
    }
}

public class PolicyholderRequestValidator : AbstractValidator<PolicyholderRequest>
{
    public PolicyholderRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.AddressLine).NotEmpty().MaximumLength(120);
        RuleFor(x => x.City).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Postcode).NotEmpty().MaximumLength(12);
        RuleFor(x => x.DateOfBirth).LessThan(new DateOnly(2008, 1, 1));
    }
}

public class PolicyRequestValidator : AbstractValidator<PolicyRequest>
{
    public PolicyRequestValidator()
    {
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        RuleFor(x => x.AnnualPremium).GreaterThan(0);
        RuleFor(x => x.SumInsured).GreaterThan(0);
        RuleFor(x => x.NoClaimsYears).InclusiveBetween(0, 60);
    }
}

public class CreateClaimRequestValidator : AbstractValidator<CreateClaimRequest>
{
    public CreateClaimRequestValidator()
    {
        RuleFor(x => x.ClaimType).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.EstimatedAmount).GreaterThan(0);
    }
}

public class UpdateClaimRequestValidator : AbstractValidator<UpdateClaimRequest>
{
    public UpdateClaimRequestValidator()
    {
        RuleFor(x => x.ClaimType).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.EstimatedAmount).GreaterThan(0);
        RuleFor(x => x.ApprovedAmount).GreaterThanOrEqualTo(0).When(x => x.ApprovedAmount.HasValue);
        RuleFor(x => x.PaidAmount).GreaterThanOrEqualTo(0).When(x => x.PaidAmount.HasValue);
    }
}

public class ChangeStatusRequestValidator : AbstractValidator<ChangeStatusRequest>
{
    public ChangeStatusRequestValidator()
    {
        RuleFor(x => x.Status).Must(s => ClaimStatuses.All.Contains(s)).WithMessage("Unknown claim status.");
        RuleFor(x => x.Reason).MaximumLength(300);
    }
}

public class ReopenClaimRequestValidator : AbstractValidator<ReopenClaimRequest>
{
    public ReopenClaimRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(300);
    }
}

public class CreateNoteRequestValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteRequestValidator()
    {
        RuleFor(x => x.NoteType).Must(t => t is "Update" or "Call" or "Assessment" or "Reserve" or "Internal");
        RuleFor(x => x.Body).NotEmpty().MaximumLength(2000);
    }
}
