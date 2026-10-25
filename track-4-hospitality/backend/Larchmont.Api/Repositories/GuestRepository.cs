using Larchmont.Api.Data;
using Larchmont.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Larchmont.Api.Repositories;

public interface IGuestRepository
{
    Task<(List<Guest> Items, int Total)> SearchAsync(string? q, string? region, int page, int pageSize);
    Task<Guest?> GetAsync(int id);
    Task<Guest?> GetByEmailAsync(string email);
    Task AddAsync(Guest guest);
    Task SaveAsync();
}

public class GuestRepository : IGuestRepository
{
    private readonly LarchmontDbContext _db;
    public GuestRepository(LarchmontDbContext db) => _db = db;

    public async Task<(List<Guest> Items, int Total)> SearchAsync(string? q, string? region, int page, int pageSize)
    {
        var query = _db.Guests.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(g => g.LastName.Contains(q) || g.FirstName.Contains(q) || g.Email.Contains(q));
        if (!string.IsNullOrWhiteSpace(region)) query = query.Where(g => g.Region == region);
        var total = await query.CountAsync();
        var items = await query.OrderBy(g => g.LastName).ThenBy(g => g.FirstName)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public Task<Guest?> GetAsync(int id) => _db.Guests.FirstOrDefaultAsync(g => g.Id == id);
    public Task<Guest?> GetByEmailAsync(string email) =>
        _db.Guests.FirstOrDefaultAsync(g => g.Email == email);
    public async Task AddAsync(Guest guest) { _db.Guests.Add(guest); await _db.SaveChangesAsync(); }
    public Task SaveAsync() => _db.SaveChangesAsync();
}
