using Larchmont.Api.Data;
using Larchmont.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Larchmont.Api.Repositories;

public interface IRateRepository
{
    Task<List<RatePlan>> ListPlansAsync();
    Task<RatePlan?> GetPlanAsync(int id);
    Task<RatePlan?> GetPlanByCodeAsync(string code);
    Task<List<RoomRate>> ListRatesAsync(int? hotelId, int? roomTypeId);
    Task<RoomRate?> GetRateAsync(int id);
    Task<RoomRate?> FindRateAsync(int hotelId, int roomTypeId, int ratePlanId, DateOnly date);
    Task AddRateAsync(RoomRate rate);
    Task SaveAsync();
}

public class RateRepository : IRateRepository
{
    private readonly LarchmontDbContext _db;
    public RateRepository(LarchmontDbContext db) => _db = db;

    public Task<List<RatePlan>> ListPlansAsync() => _db.RatePlans.OrderBy(p => p.Id).ToListAsync();
    public Task<RatePlan?> GetPlanAsync(int id) => _db.RatePlans.FirstOrDefaultAsync(p => p.Id == id);
    public Task<RatePlan?> GetPlanByCodeAsync(string code) => _db.RatePlans.FirstOrDefaultAsync(p => p.Code == code);

    public Task<List<RoomRate>> ListRatesAsync(int? hotelId, int? roomTypeId)
    {
        var q = _db.RoomRates.AsQueryable();
        if (hotelId.HasValue) q = q.Where(r => r.HotelId == hotelId);
        if (roomTypeId.HasValue) q = q.Where(r => r.RoomTypeId == roomTypeId);
        return q.OrderBy(r => r.HotelId).ThenBy(r => r.RoomTypeId).ThenBy(r => r.RatePlanId).ToListAsync();
    }

    public Task<RoomRate?> GetRateAsync(int id) => _db.RoomRates.FirstOrDefaultAsync(r => r.Id == id);

    public Task<RoomRate?> FindRateAsync(int hotelId, int roomTypeId, int ratePlanId, DateOnly date) =>
        _db.RoomRates.Where(r => r.HotelId == hotelId && r.RoomTypeId == roomTypeId && r.RatePlanId == ratePlanId
                && r.ValidFrom <= date && r.ValidTo >= date)
            .OrderByDescending(r => r.ValidFrom).FirstOrDefaultAsync();

    public async Task AddRateAsync(RoomRate rate) { _db.RoomRates.Add(rate); await _db.SaveChangesAsync(); }
    public Task SaveAsync() => _db.SaveChangesAsync();
}
