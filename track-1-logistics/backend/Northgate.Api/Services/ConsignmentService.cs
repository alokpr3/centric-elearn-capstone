using Northgate.Api.Domain;
using Northgate.Api.Dtos;
using Northgate.Api.Infrastructure;
using Northgate.Api.Mappers;
using Northgate.Api.Models;
using Northgate.Api.Repositories;

namespace Northgate.Api.Services;

public class ConsignmentService
{
    private readonly IConsignmentRepository _consignments;
    private readonly ICustomerRepository _customers;
    private readonly IDepotRepository _depots;
    private readonly IDriverRepository _drivers;

    public ConsignmentService(IConsignmentRepository consignments, ICustomerRepository customers, IDepotRepository depots, IDriverRepository drivers)
    {
        _consignments = consignments; _customers = customers; _depots = depots; _drivers = drivers;
    }

    public async Task<PagedResult<ConsignmentListItemDto>> SearchAsync(ConsignmentFilter filter)
    {
        filter = filter with { Page = Math.Max(1, filter.Page), PageSize = Math.Clamp(filter.PageSize, 1, 200) };
        var (items, total) = await _consignments.SearchAsync(filter);
        var rows = new List<ConsignmentListItemDto>();
        foreach (var c in items)
        {
            var customer = await _customers.GetAsync(c.CustomerId);
            var origin = await _depots.GetAsync(c.OriginDepotId);
            var dest = await _depots.GetAsync(c.DestDepotId);
            var driver = c.DriverId.HasValue ? await _drivers.GetAsync(c.DriverId.Value) : null;
            rows.Add(ConsignmentMapper.ToListItem(c, customer?.Name ?? "", origin?.Name ?? "", dest?.Name ?? "", driver?.Name));
        }
        return new PagedResult<ConsignmentListItemDto>(rows, total, filter.Page, filter.PageSize);
    }

    public async Task<ConsignmentDetailDto> GetAsync(int id)
    {
        var c = await _consignments.GetAsync(id) ?? throw new NotFoundException($"Consignment {id} not found.");
        return await ToDetailAsync(c);
    }

    public async Task<ConsignmentDetailDto> GetByReferenceAsync(string reference)
    {
        var c = await _consignments.GetByReferenceAsync(reference.Trim().ToUpperInvariant())
                ?? throw new NotFoundException($"Consignment {reference} not found.");
        return await ToDetailAsync(c);
    }

    public async Task<ConsignmentDetailDto> CreateAsync(CreateConsignmentDto dto)
    {
        var customer = await _customers.GetAsync(dto.CustomerId) ?? throw new NotFoundException($"Customer {dto.CustomerId} not found.");
        if (!customer.IsActive) throw new BusinessRuleException("Customer account is not active.");
        var origin = await _depots.GetAsync(dto.OriginDepotId) ?? throw new NotFoundException($"Depot {dto.OriginDepotId} not found.");
        _ = await _depots.GetAsync(dto.DestDepotId) ?? throw new NotFoundException($"Depot {dto.DestDepotId} not found.");

        var now = DateTime.Now;
        var consignment = new Consignment
        {
            Reference = await _consignments.NextReferenceAsync(),
            CustomerId = dto.CustomerId,
            OriginDepotId = dto.OriginDepotId,
            DestDepotId = dto.DestDepotId,
            RegionCode = origin.RegionCode,
            ServiceLevel = dto.ServiceLevel,
            Status = ConsignmentStatus.Booked,
            Parcels = dto.Parcels,
            WeightKg = dto.WeightKg,
            RecipientName = dto.RecipientName.Trim(),
            RecipientPhone = dto.RecipientPhone.Trim(),
            DeliveryAddress = dto.DeliveryAddress.Trim(),
            DeliveryPostcode = dto.DeliveryPostcode.Trim().ToUpperInvariant(),
            BookedAt = now,
            PromisedBy = PromiseCalculator.Calculate(now, dto.ServiceLevel),
            CreatedAt = now,
            UpdatedAt = now
        };
        await _consignments.AddAsync(consignment);
        await _consignments.SaveAsync();
        return await ToDetailAsync(consignment);
    }

    public async Task<ConsignmentDetailDto> UpdateAsync(int id, UpdateConsignmentDto dto)
    {
        var c = await _consignments.GetAsync(id) ?? throw new NotFoundException($"Consignment {id} not found.");
        if (c.Status is ConsignmentStatus.Delivered or ConsignmentStatus.Cancelled)
            throw new BusinessRuleException($"A {c.Status} consignment cannot be edited.");
        _ = await _depots.GetAsync(dto.DestDepotId) ?? throw new NotFoundException($"Depot {dto.DestDepotId} not found.");
        c.Parcels = dto.Parcels;
        c.WeightKg = dto.WeightKg;
        c.RecipientName = dto.RecipientName.Trim();
        c.RecipientPhone = dto.RecipientPhone.Trim();
        c.DeliveryAddress = dto.DeliveryAddress.Trim();
        c.DeliveryPostcode = dto.DeliveryPostcode.Trim().ToUpperInvariant();
        c.DestDepotId = dto.DestDepotId;
        c.UpdatedAt = DateTime.UtcNow;
        await _consignments.SaveAsync();
        return await ToDetailAsync(c);
    }

    public async Task<ConsignmentDetailDto> CancelAsync(int id, string reason)
    {
        var c = await _consignments.GetAsync(id) ?? throw new NotFoundException($"Consignment {id} not found.");
        if (c.Status == ConsignmentStatus.Cancelled) throw new ConflictException("Consignment is already cancelled.");
        if (c.Status == ConsignmentStatus.Delivered) throw new BusinessRuleException("A delivered consignment cannot be cancelled.");
        c.Status = ConsignmentStatus.Cancelled;
        c.CancelledAt = DateTime.UtcNow;
        c.UpdatedAt = DateTime.UtcNow;
        await _consignments.SaveAsync();
        return await ToDetailAsync(c);
    }

    public async Task<List<NoteDto>> ListNotesAsync(int id)
    {
        _ = await _consignments.GetAsync(id) ?? throw new NotFoundException($"Consignment {id} not found.");
        return (await _consignments.ListNotesAsync(id)).Select(ConsignmentMapper.ToDto).ToList();
    }

    public async Task<NoteDto> AddNoteAsync(int id, CreateNoteDto dto)
    {
        _ = await _consignments.GetAsync(id) ?? throw new NotFoundException($"Consignment {id} not found.");
        var note = new ConsignmentNote
        {
            ConsignmentId = id, AuthorUserId = dto.AuthorUserId, NoteType = dto.NoteType, Note = dto.Note.Trim(), CreatedAt = DateTime.UtcNow
        };
        await _consignments.AddNoteAsync(note);
        await _consignments.SaveAsync();
        return ConsignmentMapper.ToDto(note);
    }

    public async Task<List<EventDto>> ListEventsAsync(int id)
    {
        _ = await _consignments.GetAsync(id) ?? throw new NotFoundException($"Consignment {id} not found.");
        var events = await _consignments.ListEventsAsync(id);
        var result = new List<EventDto>();
        foreach (var e in events)
        {
            var depot = await _depots.GetAsync(e.DepotId);
            result.Add(new EventDto(e.Id, e.EventType, e.DepotId, depot?.Name ?? "", e.OccurredAt));
        }
        return result;
    }

    private async Task<ConsignmentDetailDto> ToDetailAsync(Consignment c)
    {
        var customer = await _customers.GetAsync(c.CustomerId);
        var origin = await _depots.GetAsync(c.OriginDepotId);
        var dest = await _depots.GetAsync(c.DestDepotId);
        var driver = c.DriverId.HasValue ? await _drivers.GetAsync(c.DriverId.Value) : null;
        return ConsignmentMapper.ToDetail(c, customer?.Name ?? "", origin?.Name ?? "", dest?.Name ?? "", driver?.Name);
    }
}
