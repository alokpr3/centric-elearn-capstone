using Harbourline.Api.Data;
using Harbourline.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Harbourline.Api.Repositories;

public interface IRegionRepository
{
    Task<List<Region>> ListAsync();
    Task<Region?> GetByCodeAsync(string code);
}

public class RegionRepository : IRegionRepository
{
    private readonly HarbourlineDbContext _db;
    public RegionRepository(HarbourlineDbContext db) => _db = db;
    public Task<List<Region>> ListAsync() => _db.Regions.AsNoTracking().OrderBy(r => r.Id).ToListAsync();
    public Task<Region?> GetByCodeAsync(string code) => _db.Regions.AsNoTracking().FirstOrDefaultAsync(r => r.Code == code);
}

public interface IProductRepository
{
    Task<List<Product>> ListAsync(string? line, bool? active);
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByCodeAsync(string code);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
}

public class ProductRepository : IProductRepository
{
    private readonly HarbourlineDbContext _db;
    public ProductRepository(HarbourlineDbContext db) => _db = db;

    public Task<List<Product>> ListAsync(string? line, bool? active)
    {
        var q = _db.Products.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(line)) q = q.Where(p => p.Line == line);
        if (active.HasValue) q = q.Where(p => p.Active == active.Value);
        return q.OrderBy(p => p.Code).ToListAsync();
    }

    public Task<Product?> GetByIdAsync(int id) => _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    public Task<Product?> GetByCodeAsync(string code) => _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Code == code);

    public async Task<Product> AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }
}

public interface IUserRepository
{
    Task<List<AppUser>> ListAsync();
    Task<AppUser?> GetByIdAsync(int id);
    Task<AppUser?> GetByEmailAsync(string email);
    Task UpdateAsync(AppUser user);
}

public class UserRepository : IUserRepository
{
    private readonly HarbourlineDbContext _db;
    public UserRepository(HarbourlineDbContext db) => _db = db;
    public Task<List<AppUser>> ListAsync() => _db.Users.AsNoTracking().OrderBy(u => u.Id).ToListAsync();
    public Task<AppUser?> GetByIdAsync(int id) => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    public Task<AppUser?> GetByEmailAsync(string email) => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public async Task UpdateAsync(AppUser user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}

public interface IHandlerRepository
{
    Task<List<Handler>> ListAsync(int? regionId, bool? active);
    Task<Handler?> GetByIdAsync(int id);
    Task<Handler?> GetByUserIdAsync(int userId);
    Task<List<HandlerGrade>> ListGradesAsync();
    Task<Handler> AddAsync(Handler handler);
    Task UpdateAsync(Handler handler);
}

public class HandlerRepository : IHandlerRepository
{
    private readonly HarbourlineDbContext _db;
    public HandlerRepository(HarbourlineDbContext db) => _db = db;

    public Task<List<Handler>> ListAsync(int? regionId, bool? active)
    {
        var q = _db.Handlers.AsNoTracking().Include(h => h.Grade).Include(h => h.Region).AsQueryable();
        if (regionId.HasValue) q = q.Where(h => h.RegionId == regionId.Value);
        if (active.HasValue) q = q.Where(h => h.Active == active.Value);
        return q.OrderBy(h => h.Name).ToListAsync();
    }

    public Task<Handler?> GetByIdAsync(int id) =>
        _db.Handlers.AsNoTracking().Include(h => h.Grade).Include(h => h.Region).FirstOrDefaultAsync(h => h.Id == id);

    public Task<Handler?> GetByUserIdAsync(int userId) =>
        _db.Handlers.AsNoTracking().Include(h => h.Grade).Include(h => h.Region).FirstOrDefaultAsync(h => h.UserId == userId);

    public Task<List<HandlerGrade>> ListGradesAsync() => _db.HandlerGrades.AsNoTracking().OrderBy(g => g.ApprovalLimit).ToListAsync();

    public async Task<Handler> AddAsync(Handler handler)
    {
        _db.Handlers.Add(handler);
        await _db.SaveChangesAsync();
        return handler;
    }

    public async Task UpdateAsync(Handler handler)
    {
        _db.Entry(handler).State = EntityState.Modified;
        await _db.SaveChangesAsync();
    }
}

public interface IPolicyholderRepository
{
    Task<(List<Policyholder> Items, int Total)> ListAsync(string? search, int? regionId, int page, int pageSize);
    Task<Policyholder?> GetByIdAsync(int id);
    Task<Policyholder> AddAsync(Policyholder holder);
    Task UpdateAsync(Policyholder holder);
}

public class PolicyholderRepository : IPolicyholderRepository
{
    private readonly HarbourlineDbContext _db;
    public PolicyholderRepository(HarbourlineDbContext db) => _db = db;

    public async Task<(List<Policyholder> Items, int Total)> ListAsync(string? search, int? regionId, int page, int pageSize)
    {
        var q = _db.Policyholders.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p => p.FullName.Contains(search) || p.Email.Contains(search) || p.Postcode.Contains(search));
        if (regionId.HasValue) q = q.Where(p => p.RegionId == regionId.Value);
        var total = await q.CountAsync();
        var items = await q.OrderBy(p => p.FullName).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public Task<Policyholder?> GetByIdAsync(int id) => _db.Policyholders.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Policyholder> AddAsync(Policyholder holder)
    {
        _db.Policyholders.Add(holder);
        await _db.SaveChangesAsync();
        return holder;
    }

    public async Task UpdateAsync(Policyholder holder)
    {
        _db.Entry(holder).State = EntityState.Modified;
        await _db.SaveChangesAsync();
    }
}

public interface IPolicyRepository
{
    Task<(List<Policy> Items, int Total)> ListAsync(string? status, int? productId, int? regionId, int page, int pageSize);
    Task<List<Policy>> ListForHolderAsync(int holderId);
    Task<Policy?> GetByIdAsync(int id);
    Task<Policy?> GetByNumberAsync(string policyNumber);
    Task<int> CountAsync();
    Task<Policy> AddAsync(Policy policy);
    Task UpdateAsync(Policy policy);
}

public class PolicyRepository : IPolicyRepository
{
    private readonly HarbourlineDbContext _db;
    public PolicyRepository(HarbourlineDbContext db) => _db = db;

    public async Task<(List<Policy> Items, int Total)> ListAsync(string? status, int? productId, int? regionId, int page, int pageSize)
    {
        var q = _db.Policies.AsNoTracking().Include(p => p.Policyholder).Include(p => p.Product).AsQueryable();
        if (!string.IsNullOrEmpty(status)) q = q.Where(p => p.Status == status);
        if (productId.HasValue) q = q.Where(p => p.ProductId == productId.Value);
        if (regionId.HasValue) q = q.Where(p => p.RegionId == regionId.Value);
        var total = await q.CountAsync();
        var items = await q.OrderBy(p => p.PolicyNumber).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public Task<List<Policy>> ListForHolderAsync(int holderId) =>
        _db.Policies.AsNoTracking().Include(p => p.Policyholder).Include(p => p.Product)
            .Where(p => p.PolicyholderId == holderId).OrderBy(p => p.StartDate).ToListAsync();

    public Task<Policy?> GetByIdAsync(int id) =>
        _db.Policies.AsNoTracking().Include(p => p.Policyholder).Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == id);

    public Task<Policy?> GetByNumberAsync(string policyNumber) =>
        _db.Policies.AsNoTracking().Include(p => p.Policyholder).Include(p => p.Product).FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber);

    public Task<int> CountAsync() => _db.Policies.CountAsync();

    public async Task<Policy> AddAsync(Policy policy)
    {
        _db.Policies.Add(policy);
        await _db.SaveChangesAsync();
        return policy;
    }

    public async Task UpdateAsync(Policy policy)
    {
        _db.Entry(policy).State = EntityState.Modified;
        await _db.SaveChangesAsync();
    }
}

public class ClaimFilter
{
    public string? Status { get; set; }
    public int? HandlerId { get; set; }
    public int? RegionId { get; set; }
    public int? PolicyId { get; set; }
    public int? PolicyholderId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public interface IClaimRepository
{
    Task<List<Claim>> ListAsync(ClaimFilter filter);
    Task<int> CountAsync(ClaimFilter filter);
    Task<Claim?> GetByIdAsync(int id);
    Task<int> CountOpenForHandlerAsync(int handlerId);
    Task<int> NextSequenceAsync();
    Task<Claim?> GetByExternalRefAsync(string externalRef);
    Task<Claim> AddAsync(Claim claim);
    Task UpdateAsync(Claim claim);
    Task AddHistoryAsync(ClaimStatusHistory entry);
    Task<List<ClaimStatusHistory>> ListHistoryAsync(int claimId);
}

public class ClaimRepository : IClaimRepository
{
    private readonly HarbourlineDbContext _db;
    public ClaimRepository(HarbourlineDbContext db) => _db = db;

    private IQueryable<Claim> Filtered(ClaimFilter f)
    {
        var q = _db.Claims.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(f.Status)) q = q.Where(c => c.Status == f.Status);
        if (f.HandlerId.HasValue) q = q.Where(c => c.HandlerId == f.HandlerId.Value);
        if (f.RegionId.HasValue) q = q.Where(c => c.RegionId == f.RegionId.Value);
        if (f.PolicyId.HasValue) q = q.Where(c => c.PolicyId == f.PolicyId.Value);
        if (f.PolicyholderId.HasValue)
            q = q.Where(c => _db.Policies.Any(p => p.Id == c.PolicyId && p.PolicyholderId == f.PolicyholderId.Value));
        return q;
    }

    public Task<List<Claim>> ListAsync(ClaimFilter f) =>
        Filtered(f).OrderByDescending(c => c.ReportedDate).ThenByDescending(c => c.Id).Skip((f.Page - 1) * f.PageSize).Take(f.PageSize).ToListAsync();

    public Task<int> CountAsync(ClaimFilter f) => Filtered(f).CountAsync();

    public Task<Claim?> GetByIdAsync(int id) => _db.Claims.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public Task<int> CountOpenForHandlerAsync(int handlerId) =>
        _db.Claims.CountAsync(c => c.HandlerId == handlerId && ClaimStatuses.Open.Contains(c.Status));

    public async Task<int> NextSequenceAsync() => (await _db.Claims.MaxAsync(c => (int?)c.Id) ?? 0) + 1;

    public Task<Claim?> GetByExternalRefAsync(string externalRef) =>
        _db.Claims.AsNoTracking().FirstOrDefaultAsync(c => c.ExternalRef == externalRef);

    public async Task<Claim> AddAsync(Claim claim)
    {
        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();
        return claim;
    }

    public async Task UpdateAsync(Claim claim)
    {
        _db.Entry(claim).State = EntityState.Modified;
        await _db.SaveChangesAsync();
    }

    public async Task AddHistoryAsync(ClaimStatusHistory entry)
    {
        _db.ClaimStatusHistory.Add(entry);
        await _db.SaveChangesAsync();
    }

    public Task<List<ClaimStatusHistory>> ListHistoryAsync(int claimId) =>
        _db.ClaimStatusHistory.AsNoTracking().Where(h => h.ClaimId == claimId).OrderBy(h => h.ChangedAt).ThenBy(h => h.Id).ToListAsync();
}

public interface IClaimNoteRepository
{
    Task<List<ClaimNote>> ListForClaimAsync(int claimId);
    Task<int> CountForClaimAsync(int claimId);
    Task<DateTime?> LastNoteAtAsync(int claimId);
    Task<ClaimNote?> GetByIdAsync(int id);
    Task<ClaimNote> AddAsync(ClaimNote note);
    Task DeleteAsync(ClaimNote note);
}

public class ClaimNoteRepository : IClaimNoteRepository
{
    private readonly HarbourlineDbContext _db;
    public ClaimNoteRepository(HarbourlineDbContext db) => _db = db;

    public Task<List<ClaimNote>> ListForClaimAsync(int claimId) =>
        _db.ClaimNotes.AsNoTracking().Where(n => n.ClaimId == claimId).OrderByDescending(n => n.CreatedAt).ToListAsync();

    public Task<int> CountForClaimAsync(int claimId) => _db.ClaimNotes.CountAsync(n => n.ClaimId == claimId);

    public async Task<DateTime?> LastNoteAtAsync(int claimId) =>
        await _db.ClaimNotes.Where(n => n.ClaimId == claimId).MaxAsync(n => (DateTime?)n.CreatedAt);

    public Task<ClaimNote?> GetByIdAsync(int id) => _db.ClaimNotes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);

    public async Task<ClaimNote> AddAsync(ClaimNote note)
    {
        _db.ClaimNotes.Add(note);
        await _db.SaveChangesAsync();
        return note;
    }

    public async Task DeleteAsync(ClaimNote note)
    {
        _db.ClaimNotes.Remove(note);
        await _db.SaveChangesAsync();
    }
}

public interface IImportBatchRepository
{
    Task<List<ImportBatch>> ListAsync();
    Task<ImportBatch?> GetByIdAsync(int id);
    Task<ImportBatch> AddAsync(ImportBatch batch);
}

public class ImportBatchRepository : IImportBatchRepository
{
    private readonly HarbourlineDbContext _db;
    public ImportBatchRepository(HarbourlineDbContext db) => _db = db;
    public Task<List<ImportBatch>> ListAsync() => _db.ImportBatches.AsNoTracking().OrderByDescending(b => b.CreatedAt).ToListAsync();
    public Task<ImportBatch?> GetByIdAsync(int id) => _db.ImportBatches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    public async Task<ImportBatch> AddAsync(ImportBatch batch)
    {
        _db.ImportBatches.Add(batch);
        await _db.SaveChangesAsync();
        return batch;
    }
}

public interface IReportRepository
{
    Task<List<LossRatioProductRow>> LossRatioAsync(DateTime periodStart, DateTime toExclusive);
    Task<List<AgeingProjection>> AgeingAsync();
    Task<List<WorkloadProjection>> WorkloadAsync();
    Task<List<SummaryProjection>> SummaryAsync();
}

public record LossRatioProductRow(string ProductCode, string ProductName, decimal WrittenPremium, decimal PaidClaims, int ClaimCount);
public record AgeingProjection(string Status, int ClaimId, DateTime? EnteredStatusAt);
public record WorkloadProjection(int HandlerId, string HandlerName, string GradeCode, int MaxOpenClaims, int OpenClaims, decimal OpenEstimatedValue);
public record SummaryProjection(string RegionCode, string Status, int Claims, decimal EstimatedTotal);

public class ReportRepository : IReportRepository
{
    private readonly HarbourlineDbContext _db;
    public ReportRepository(HarbourlineDbContext db) => _db = db;

    public async Task<List<LossRatioProductRow>> LossRatioAsync(DateTime periodStart, DateTime toExclusive)
    {
        var fromDate = DateOnly.FromDateTime(periodStart);
        var toDate = DateOnly.FromDateTime(toExclusive);

        var premiums = await _db.Policies.AsNoTracking()
            .Where(p => p.StartDate >= fromDate && p.StartDate < toDate && p.Status != "Cancelled")
            .GroupBy(p => p.ProductId)
            .Select(g => new { ProductId = g.Key, Premium = g.Sum(p => p.AnnualPremium) })
            .ToListAsync();

        var settlements = await (
            from h in _db.ClaimStatusHistory.AsNoTracking()
            join c in _db.Claims.AsNoTracking() on h.ClaimId equals c.Id
            join p in _db.Policies.AsNoTracking() on c.PolicyId equals p.Id
            where h.ToStatus == ClaimStatuses.Settled && h.ChangedAt >= periodStart && h.ChangedAt < toExclusive
            select new { p.ProductId, Paid = c.PaidAmount ?? 0m }
        ).ToListAsync();

        var paidByProduct = settlements.GroupBy(s => s.ProductId)
            .ToDictionary(g => g.Key, g => (Paid: g.Sum(x => x.Paid), Count: g.Count()));

        var products = await _db.Products.AsNoTracking().OrderBy(p => p.Code).ToListAsync();
        return products.Select(p =>
        {
            var premium = premiums.FirstOrDefault(x => x.ProductId == p.Id)?.Premium ?? 0m;
            paidByProduct.TryGetValue(p.Id, out var paid);
            return new LossRatioProductRow(p.Code, p.Name, premium, paid.Paid, paid.Count);
        }).ToList();
    }

    public async Task<List<AgeingProjection>> AgeingAsync()
    {
        var open = ClaimStatuses.Open;
        return await _db.Claims.AsNoTracking()
            .Where(c => open.Contains(c.Status))
            .Select(c => new AgeingProjection(c.Status, c.Id,
                _db.ClaimStatusHistory.Where(h => h.ClaimId == c.Id && h.ToStatus == c.Status).Max(h => (DateTime?)h.ChangedAt)))
            .ToListAsync();
    }

    public async Task<List<WorkloadProjection>> WorkloadAsync()
    {
        var open = ClaimStatuses.Open;
        return await _db.Handlers.AsNoTracking().Where(h => h.Active).OrderBy(h => h.Name)
            .Select(h => new WorkloadProjection(h.Id, h.Name, h.Grade!.Code, h.MaxOpenClaims,
                _db.Claims.Count(c => c.HandlerId == h.Id && open.Contains(c.Status)),
                _db.Claims.Where(c => c.HandlerId == h.Id && open.Contains(c.Status)).Sum(c => (decimal?)c.EstimatedAmount) ?? 0m))
            .ToListAsync();
    }

    public async Task<List<SummaryProjection>> SummaryAsync()
    {
        var rows = await _db.Claims.AsNoTracking()
            .GroupBy(c => new { c.RegionId, c.Status })
            .Select(g => new { g.Key.RegionId, g.Key.Status, Claims = g.Count(), Total = g.Sum(c => c.EstimatedAmount) })
            .ToListAsync();
        var regions = await _db.Regions.AsNoTracking().ToDictionaryAsync(r => r.Id, r => r.Code);
        return rows.OrderBy(r => r.RegionId).ThenBy(r => r.Status)
            .Select(r => new SummaryProjection(regions[r.RegionId], r.Status, r.Claims, r.Total)).ToList();
    }
}
