using FluentValidation;
using Northgate.Api.Dtos;
using Northgate.Api.Models;

namespace Northgate.Api.Validators;

public class DepotWriteValidator : AbstractValidator<DepotWriteDto>
{
    public DepotWriteValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        RuleFor(x => x.RegionCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.City).NotEmpty().MaximumLength(60);
        RuleFor(x => x.DailyCapacity).GreaterThan(0);
    }
}

public class CustomerWriteValidator : AbstractValidator<CustomerWriteDto>
{
    public CustomerWriteValidator()
    {
        RuleFor(x => x.AccountNo).NotEmpty().MaximumLength(12);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.AddressLine).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Postcode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0);
    }
}

public class DriverWriteValidator : AbstractValidator<DriverWriteDto>
{
    public DriverWriteValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        RuleFor(x => x.LicenceNo).NotEmpty().MaximumLength(24);
        RuleFor(x => x.VehicleReg).NotEmpty().MaximumLength(10);
        RuleFor(x => x.MaxStops).InclusiveBetween(1, 60);
    }
}

public class CreateConsignmentValidator : AbstractValidator<CreateConsignmentDto>
{
    public CreateConsignmentValidator()
    {
        RuleFor(x => x.ServiceLevel).Must(s => ServiceLevels.All.Contains(s)).WithMessage("Unknown service level.");
        RuleFor(x => x.Parcels).InclusiveBetween(1, 500);
        RuleFor(x => x.WeightKg).GreaterThan(0).LessThanOrEqualTo(25000);
        RuleFor(x => x.RecipientName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.RecipientPhone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.DeliveryAddress).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DeliveryPostcode).NotEmpty().MaximumLength(10);
    }
}

public class UpdateConsignmentValidator : AbstractValidator<UpdateConsignmentDto>
{
    public UpdateConsignmentValidator()
    {
        RuleFor(x => x.Parcels).InclusiveBetween(1, 500);
        RuleFor(x => x.WeightKg).GreaterThan(0).LessThanOrEqualTo(25000);
        RuleFor(x => x.RecipientName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DeliveryAddress).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DeliveryPostcode).NotEmpty().MaximumLength(10);
    }
}

public class CreateNoteValidator : AbstractValidator<CreateNoteDto>
{
    public CreateNoteValidator()
    {
        RuleFor(x => x.Note).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.NoteType).Must(t => t is "NOTE" or "EXCEPTION");
    }
}

public class CancelConsignmentValidator : AbstractValidator<CancelConsignmentDto>
{
    public CancelConsignmentValidator() => RuleFor(x => x.Reason).NotEmpty().MaximumLength(200);
}
