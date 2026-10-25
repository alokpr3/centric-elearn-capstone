namespace Northgate.Api.Models;

public static class ConsignmentStatus
{
    public const string Booked = "BOOKED";
    public const string Collected = "COLLECTED";
    public const string AtDepot = "AT_DEPOT";
    public const string InTransit = "IN_TRANSIT";
    public const string OutForDelivery = "OUT_FOR_DELIVERY";
    public const string Delivered = "DELIVERED";
    public const string FailedDelivery = "FAILED_DELIVERY";
    public const string Cancelled = "CANCELLED";

    public static readonly string[] All =
        { Booked, Collected, AtDepot, InTransit, OutForDelivery, Delivered, FailedDelivery, Cancelled };
}

public static class ServiceLevels
{
    public const string NextDay = "NEXT_DAY";
    public const string ExpressAm = "EXPRESS_AM";
    public const string Economy = "ECONOMY";
    public const string Pallet48 = "PALLET_48";
    public const string SameDay = "SAME_DAY";

    public static readonly string[] All = { NextDay, ExpressAm, Economy, Pallet48, SameDay };
}

public class Depot
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string RegionCode { get; set; } = "";
    public string City { get; set; } = "";
    public int DailyCapacity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string AccountNo { get; set; } = "";
    public string Name { get; set; } = "";
    public string ContactName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string AddressLine { get; set; } = "";
    public string City { get; set; } = "";
    public string Postcode { get; set; } = "";
    public string RegionCode { get; set; } = "";
    public decimal CreditLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "";
    public string? RegionCode { get; set; }
    public int? DepotId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class Driver
{
    public int Id { get; set; }
    public int DepotId { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string LicenceNo { get; set; } = "";
    public string VehicleReg { get; set; } = "";
    public int MaxStops { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class Consignment
{
    public int Id { get; set; }
    public string Reference { get; set; } = "";
    public int CustomerId { get; set; }
    public int OriginDepotId { get; set; }
    public int DestDepotId { get; set; }
    public string RegionCode { get; set; } = "";
    public string ServiceLevel { get; set; } = "";
    public string Status { get; set; } = ConsignmentStatus.Booked;
    public int Parcels { get; set; }
    public decimal WeightKg { get; set; }
    public string RecipientName { get; set; } = "";
    public string RecipientPhone { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";
    public string DeliveryPostcode { get; set; } = "";
    public DateTime BookedAt { get; set; }
    public DateTime PromisedBy { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public int? DriverId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Customer? Customer { get; set; }
    public Depot? OriginDepot { get; set; }
    public Depot? DestDepot { get; set; }
    public Driver? Driver { get; set; }
}

public class ConsignmentNote
{
    public int Id { get; set; }
    public int ConsignmentId { get; set; }
    public int AuthorUserId { get; set; }
    public string NoteType { get; set; } = "NOTE";
    public string Note { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class ConsignmentEvent
{
    public int Id { get; set; }
    public int ConsignmentId { get; set; }
    public string EventType { get; set; } = "";
    public int DepotId { get; set; }
    public DateTime OccurredAt { get; set; }
}

public class ImportBatch
{
    public int Id { get; set; }
    public string FileName { get; set; } = "";
    public int TotalRows { get; set; }
    public int ImportedRows { get; set; }
    public int SkippedRows { get; set; }
    public string? Summary { get; set; }
    public DateTime CreatedAt { get; set; }
}
