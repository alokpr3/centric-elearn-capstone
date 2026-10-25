using Microsoft.EntityFrameworkCore;
using Northgate.Api.Data;
using Northgate.Api.Dtos;
using Northgate.Api.Models;

namespace Northgate.Api.Repositories;

public class DepotRepository : IDepotRepository
{
    private readonly NorthgateDbContext _db;
    public DepotRepository(NorthgateDbContext db) => _db = db;

    public Task<List<Depot>> ListAsync(string? regionCode) =>
        _db.Depots.AsNoTracking().Where(d => regionCode == null || d.RegionCode == regionCode).OrderBy(d => d.Name).ToListAsync();
    public Task<Depot?> GetAsync(int id) => _db.Depots.FirstOrDefaultAsync(d => d.Id == id);
    public Task<Depot?> GetByCodeAsync(string code) => _db.Depots.AsNoTracking().FirstOrDefaultAsync(d => d.Code == code);
    public async Task AddAsync(Depot depot) => await _db.Depots.AddAsync(depot);
    public Task SaveAsync() => _db.SaveChangesAsync();
}

public class CustomerRepository : ICustomerRepository
{
    private readonly NorthgateDbContext _db;
    public CustomerRepository(NorthgateDbContext db) => _db = db;

    public async Task<(List<Customer> Items, int Total)> SearchAsync(string? search, int page, int pageSize)
    {
        var query = _db.Customers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search) || c.AccountNo.Contains(search));
        var total = await query.CountAsync();
        var items = await query.OrderBy(c => c.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
    public Task<Customer?> GetAsync(int id) => _db.Customers.FirstOrDefaultAsync(c => c.Id == id);
    public Task<Customer?> GetByAccountNoAsync(string accountNo) => _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.AccountNo == accountNo);
    public async Task AddAsync(Customer customer) => await _db.Customers.AddAsync(customer);
    public Task SaveAsync() => _db.SaveChangesAsync();
}

public class UserRepository : IUserRepository
{
    private readonly NorthgateDbContext _db;
    public UserRepository(NorthgateDbContext db) => _db = db;

    public Task<List<AppUser>> ListAsync() => _db.Users.AsNoTracking().OrderBy(u => u.Username).ToListAsync();
    public Task<AppUser?> GetAsync(int id) => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    public Task<AppUser?> GetByUsernameAsync(string username) => _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    public Task SaveAsync() => _db.SaveChangesAsync();
}

public class DriverRepository : IDriverRepository
{
    private readonly NorthgateDbContext _db;
    public DriverRepository(NorthgateDbContext db) => _db = db;

    public Task<List<Driver>> ListAsync(int? depotId) =>
        _db.Drivers.AsNoTracking().Where(d => depotId == null || d.DepotId == depotId).OrderBy(d => d.Name).ToListAsync();
    public Task<Driver?> GetAsync(int id) => _db.Drivers.FirstOrDefaultAsync(d => d.Id == id);
    public async Task AddAsync(Driver driver) => await _db.Drivers.AddAsync(driver);
    public Task SaveAsync() => _db.SaveChangesAsync();
}

public class ConsignmentRepository : IConsignmentRepository
{
    private readonly NorthgateDbContext _db;
    public ConsignmentRepository(NorthgateDbContext db) => _db = db;

    public async Task<(List<Consignment> Items, int Total)> SearchAsync(ConsignmentFilter f)
    {
        var query = _db.Consignments.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(f.Status)) query = query.Where(c => c.Status == f.Status);
        if (f.DepotId.HasValue) query = query.Where(c => c.OriginDepotId == f.DepotId || c.DestDepotId == f.DepotId);
        if (f.CustomerId.HasValue) query = query.Where(c => c.CustomerId == f.CustomerId);
        if (f.From.HasValue) query = query.Where(c => c.BookedAt >= f.From);
        if (f.To.HasValue) query = query.Where(c => c.BookedAt < f.To);
        if (!string.IsNullOrWhiteSpace(f.Search)) query = query.Where(c => c.Reference.Contains(f.Search));
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(c => c.BookedAt).Skip((f.Page - 1) * f.PageSize).Take(f.PageSize).ToListAsync();
        return (items, total);
    }

    public Task<Consignment?> GetAsync(int id) => _db.Consignments.FirstOrDefaultAsync(c => c.Id == id);
    public Task<Consignment?> GetByReferenceAsync(string reference) => _db.Consignments.AsNoTracking().FirstOrDefaultAsync(c => c.Reference == reference);
    public Task<List<Consignment>> ListByCustomerAsync(int customerId) =>
        _db.Consignments.AsNoTracking().Where(c => c.CustomerId == customerId).OrderByDescending(c => c.BookedAt).Take(200).ToListAsync();
    public Task<List<Consignment>> ListByDriverAsync(int driverId) =>
        _db.Consignments.AsNoTracking().Where(c => c.DriverId == driverId).OrderByDescending(c => c.BookedAt).Take(200).ToListAsync();
    public Task<int> CountOpenForDriverAsync(int driverId) =>
        _db.Consignments.AsNoTracking().CountAsync(c => c.DriverId == driverId &&
            c.Status != ConsignmentStatus.Delivered && c.Status != ConsignmentStatus.Cancelled && c.Status != ConsignmentStatus.FailedDelivery);
    public Task<List<Consignment>> ListPromisedBetweenAsync(DateTime from, DateTime toExclusive, int? depotId) =>
        _db.Consignments.AsNoTracking()
            .Where(c => c.PromisedBy >= from && c.PromisedBy < toExclusive && (depotId == null || c.OriginDepotId == depotId))
            .ToListAsync();
    public Task<List<Consignment>> ListBookedBetweenAsync(DateTime from, DateTime toExclusive) =>
        _db.Consignments.AsNoTracking().Where(c => c.BookedAt >= from && c.BookedAt < toExclusive).ToListAsync();

    public async Task<string> NextReferenceAsync()
    {
        var last = await _db.Consignments.AsNoTracking().OrderByDescending(c => c.Id).Select(c => c.Reference).FirstOrDefaultAsync();
        var number = last != null && long.TryParse(last.Substring(3), out var n) ? n + 1 : 40100000;
        return $"NGF{number}";
    }

    public async Task AddAsync(Consignment consignment) => await _db.Consignments.AddAsync(consignment);
    public Task<List<ConsignmentNote>> ListNotesAsync(int consignmentId) =>
        _db.ConsignmentNotes.AsNoTracking().Where(n => n.ConsignmentId == consignmentId).OrderBy(n => n.CreatedAt).ToListAsync();
    public async Task AddNoteAsync(ConsignmentNote note) => await _db.ConsignmentNotes.AddAsync(note);
    public Task<List<ConsignmentEvent>> ListEventsAsync(int consignmentId) =>
        _db.ConsignmentEvents.AsNoTracking().Where(e => e.ConsignmentId == consignmentId).OrderBy(e => e.OccurredAt).ToListAsync();
    public Task SaveAsync() => _db.SaveChangesAsync();
}

public class ImportBatchRepository : IImportBatchRepository
{
    private readonly NorthgateDbContext _db;
    public ImportBatchRepository(NorthgateDbContext db) => _db = db;

    public Task<List<ImportBatch>> ListAsync() => _db.ImportBatches.AsNoTracking().OrderByDescending(b => b.Id).Take(100).ToListAsync();
    public Task<ImportBatch?> GetAsync(int id) => _db.ImportBatches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
    public async Task AddAsync(ImportBatch batch) => await _db.ImportBatches.AddAsync(batch);
    public Task SaveAsync() => _db.SaveChangesAsync();
}
