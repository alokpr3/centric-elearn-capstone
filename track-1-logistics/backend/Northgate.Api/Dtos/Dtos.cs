namespace Northgate.Api.Dtos;

public record DepotDto(int Id, string Code, string Name, string RegionCode, string City, int DailyCapacity, bool IsActive);
public record DepotWriteDto(string Code, string Name, string RegionCode, string City, int DailyCapacity, bool IsActive = true);

public record CustomerDto(int Id, string AccountNo, string Name, string ContactName, string Email, string Phone,
    string AddressLine, string City, string Postcode, string RegionCode, decimal CreditLimit, bool IsActive);
public record CustomerWriteDto(string AccountNo, string Name, string ContactName, string Email, string Phone,
    string AddressLine, string City, string Postcode, string RegionCode, decimal CreditLimit);

public record UserDto(int Id, string Username, string Email, string PasswordHash, string Role, string? RegionCode,
    int? DepotId, bool IsActive, DateTime? LastLoginAt);
public record LoginRequestDto(string Username, string Password);
public record LoginResultDto(int UserId, string Username, string Role, string? RegionCode);

public record DriverDto(int Id, int DepotId, string DepotName, string Name, string Phone, string LicenceNo,
    string VehicleReg, int MaxStops, int AssignedStops, bool IsActive);
public record DriverWriteDto(int DepotId, string Name, string Phone, string LicenceNo, string VehicleReg, int MaxStops);
public record AssignConsignmentDto(int ConsignmentId);

public record ConsignmentListItemDto(int Id, string Reference, string CustomerName, string OriginDepot, string DestDepot,
    string ServiceLevel, string Status, DateTime BookedAt, DateTime PromisedBy, DateTime? DeliveredAt, string? DriverName);
public record ConsignmentDetailDto(int Id, string Reference, int CustomerId, string CustomerName, int OriginDepotId,
    string OriginDepot, int DestDepotId, string DestDepot, string ServiceLevel, string Status, int Parcels, decimal WeightKg,
    string RecipientName, string RecipientPhone, string DeliveryAddress, string DeliveryPostcode, DateTime BookedAt,
    DateTime PromisedBy, DateTime? DeliveredAt, DateTime? CancelledAt, int? DriverId, string? DriverName);
public record CreateConsignmentDto(int CustomerId, int OriginDepotId, int DestDepotId, string ServiceLevel, int Parcels,
    decimal WeightKg, string RecipientName, string RecipientPhone, string DeliveryAddress, string DeliveryPostcode);
public record UpdateConsignmentDto(int Parcels, decimal WeightKg, string RecipientName, string RecipientPhone,
    string DeliveryAddress, string DeliveryPostcode, int DestDepotId);
public record CancelConsignmentDto(string Reason);
public record ConsignmentFilter(string? Status = null, int? DepotId = null, int? CustomerId = null, DateTime? From = null,
    DateTime? To = null, string? Search = null, int Page = 1, int PageSize = 50);

public record NoteDto(int Id, int ConsignmentId, int AuthorUserId, string NoteType, string Note, DateTime CreatedAt);
public record CreateNoteDto(int AuthorUserId, string NoteType, string Note);
public record EventDto(int Id, string EventType, int DepotId, string DepotName, DateTime OccurredAt);

public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);

public record DispatchOnTimeRow(int DepotId, string DepotCode, string DepotName, int Total, int OnTime, int Late, double OnTimePct);
public record DispatchOnTimeReport(DateTime From, DateTime To, int Total, int OnTime, int Late, double OnTimePct,
    IReadOnlyList<DispatchOnTimeRow> Depots);
public record DepotVolumeRow(int DepotId, string DepotName, int Booked, int Delivered, int InFlight, int Cancelled);
public record CustomerVolumeRow(int CustomerId, string CustomerName, int Consignments, int Parcels, decimal WeightKg);
public record DriverWorkloadRow(int DriverId, string DriverName, string DepotName, int AssignedStops, int MaxStops);

public record ImportBatchDto(int Id, string FileName, int TotalRows, int ImportedRows, int SkippedRows, string? Summary, DateTime CreatedAt);
public record ImportResultDto(int BatchId, int TotalRows, int ImportedRows, int SkippedRows, IReadOnlyList<string> Messages);
