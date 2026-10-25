using Larchmont.Api.Data;
using Larchmont.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Larchmont.Api.Repositories;

public interface IUserRepository
{
    Task<List<User>> ListAsync(int? hotelId, string? role);
    Task<User?> GetAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveAsync();
}

public class UserRepository : IUserRepository
{
    private readonly LarchmontDbContext _db;
    public UserRepository(LarchmontDbContext db) => _db = db;

    public Task<List<User>> ListAsync(int? hotelId, string? role)
    {
        var q = _db.Users.AsQueryable();
        if (hotelId.HasValue) q = q.Where(u => u.HotelId == hotelId);
        if (!string.IsNullOrEmpty(role)) q = q.Where(u => u.Role == role);
        return q.OrderBy(u => u.DisplayName).ToListAsync();
    }

    public Task<User?> GetAsync(int id) => _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    public Task<User?> GetByEmailAsync(string email) => _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    public async Task AddAsync(User user) { _db.Users.Add(user); await _db.SaveChangesAsync(); }
    public Task SaveAsync() => _db.SaveChangesAsync();
}
