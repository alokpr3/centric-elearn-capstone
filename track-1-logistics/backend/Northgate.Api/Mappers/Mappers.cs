using Northgate.Api.Dtos;
using Northgate.Api.Models;

namespace Northgate.Api.Mappers;

public static class DepotMapper
{
    public static DepotDto ToDto(Depot d) => new(d.Id, d.Code, d.Name, d.RegionCode, d.City, d.DailyCapacity, d.IsActive);
    public static void Apply(Depot d, DepotWriteDto w)
    {
        d.Code = w.Code.Trim().ToUpperInvariant(); d.Name = w.Name.Trim(); d.RegionCode = w.RegionCode.Trim().ToUpperInvariant();
        d.City = w.City.Trim(); d.DailyCapacity = w.DailyCapacity; d.IsActive = w.IsActive;
    }
}

public static class CustomerMapper
{
    public static CustomerDto ToDto(Customer c) => new(c.Id, c.AccountNo, c.Name, c.ContactName, c.Email, c.Phone,
        c.AddressLine, c.City, c.Postcode, c.RegionCode, c.CreditLimit, c.IsActive);
    public static void Apply(Customer c, CustomerWriteDto w)
    {
        c.AccountNo = w.AccountNo.Trim().ToUpperInvariant(); c.Name = w.Name.Trim(); c.ContactName = w.ContactName.Trim();
        c.Email = w.Email.Trim(); c.Phone = w.Phone.Trim(); c.AddressLine = w.AddressLine.Trim(); c.City = w.City.Trim();
        c.Postcode = w.Postcode.Trim().ToUpperInvariant(); c.RegionCode = w.RegionCode.Trim().ToUpperInvariant();
        c.CreditLimit = w.CreditLimit;
    }
}

public static class UserMapper
{
    public static UserDto ToDto(AppUser u) => new(u.Id, u.Username, u.Email, u.PasswordHash, u.Role, u.RegionCode, u.DepotId, u.IsActive, u.LastLoginAt);
}

public static class DriverMapper
{
    public static DriverDto ToDto(Driver d, string depotName, int assignedStops) => new(d.Id, d.DepotId, depotName, d.Name, d.Phone, d.LicenceNo,
        d.VehicleReg, d.MaxStops, assignedStops, d.IsActive);
    public static void Apply(Driver d, DriverWriteDto w)
    {
        d.DepotId = w.DepotId; d.Name = w.Name.Trim(); d.Phone = w.Phone.Trim(); d.LicenceNo = w.LicenceNo.Trim().ToUpperInvariant();
        d.VehicleReg = w.VehicleReg.Trim().ToUpperInvariant(); d.MaxStops = w.MaxStops;
    }
}

public static class ConsignmentMapper
{
    public static ConsignmentListItemDto ToListItem(Consignment c, string customer, string origin, string dest, string? driver) =>
        new(c.Id, c.Reference, customer, origin, dest, c.ServiceLevel, c.Status, c.BookedAt, c.PromisedBy, c.DeliveredAt, driver);

    public static ConsignmentDetailDto ToDetail(Consignment c, string customer, string origin, string dest, string? driver) =>
        new(c.Id, c.Reference, c.CustomerId, customer, c.OriginDepotId, origin, c.DestDepotId, dest, c.ServiceLevel, c.Status,
            c.Parcels, c.WeightKg, c.RecipientName, c.RecipientPhone, c.DeliveryAddress, c.DeliveryPostcode, c.BookedAt,
            c.PromisedBy, c.DeliveredAt, c.CancelledAt, c.DriverId, driver);

    public static NoteDto ToDto(ConsignmentNote n) => new(n.Id, n.ConsignmentId, n.AuthorUserId, n.NoteType, n.Note, n.CreatedAt);
}
