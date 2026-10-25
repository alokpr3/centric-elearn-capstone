using Northgate.Api.Dtos;
using Northgate.Api.Models;
using Northgate.Api.Repositories;

namespace Northgate.Api.Services;

public class ReportService
{
    private readonly IConsignmentRepository _consignments;
    private readonly IDepotRepository _depots;
    private readonly ICustomerRepository _customers;
    private readonly IDriverRepository _drivers;

    public ReportService(IConsignmentRepository consignments, IDepotRepository depots, ICustomerRepository customers, IDriverRepository drivers)
    {
        _consignments = consignments; _depots = depots; _customers = customers; _drivers = drivers;
    }

    public async Task<DispatchOnTimeReport> DispatchOnTimeAsync(DateTime from, DateTime to, int? depotId)
    {
        var toExclusive = to.Date.AddDays(1);
        var rows = await _consignments.ListPromisedBetweenAsync(from.Date, toExclusive, depotId);
        var depots = (await _depots.ListAsync(null)).ToDictionary(d => d.Id);

        var perDepot = rows.GroupBy(c => c.OriginDepotId).Select(g =>
        {
            var total = g.Count();
            var onTime = g.Count(c => c.Status == ConsignmentStatus.Delivered && c.DeliveredAt <= c.PromisedBy);
            var depot = depots[g.Key];
            return new DispatchOnTimeRow(g.Key, depot.Code, depot.Name, total, onTime, total - onTime, Pct(onTime, total));
        }).OrderBy(r => r.DepotName).ToList();

        var networkTotal = perDepot.Sum(r => r.Total);
        var networkOnTime = perDepot.Sum(r => r.OnTime);
        return new DispatchOnTimeReport(from.Date, to.Date, networkTotal, networkOnTime, networkTotal - networkOnTime,
            Pct(networkOnTime, networkTotal), perDepot);
    }

    public async Task<List<DepotVolumeRow>> DepotVolumesAsync(DateTime from, DateTime to)
    {
        var rows = await _consignments.ListBookedBetweenAsync(from.Date, to.Date.AddDays(1));
        var depots = await _depots.ListAsync(null);
        return depots.Select(d =>
        {
            var mine = rows.Where(c => c.OriginDepotId == d.Id).ToList();
            return new DepotVolumeRow(d.Id, d.Name, mine.Count,
                mine.Count(c => c.Status == ConsignmentStatus.Delivered),
                mine.Count(c => c.Status is not (ConsignmentStatus.Delivered or ConsignmentStatus.Cancelled or ConsignmentStatus.FailedDelivery)),
                mine.Count(c => c.Status == ConsignmentStatus.Cancelled));
        }).Where(r => r.Booked > 0).OrderByDescending(r => r.Booked).ToList();
    }

    public async Task<List<CustomerVolumeRow>> CustomerVolumesAsync(DateTime from, DateTime to, int top)
    {
        var rows = await _consignments.ListBookedBetweenAsync(from.Date, to.Date.AddDays(1));
        var result = new List<CustomerVolumeRow>();
        foreach (var g in rows.GroupBy(c => c.CustomerId).OrderByDescending(g => g.Count()).Take(Math.Clamp(top, 1, 100)))
        {
            var customer = await _customers.GetAsync(g.Key);
            result.Add(new CustomerVolumeRow(g.Key, customer?.Name ?? "", g.Count(), g.Sum(c => c.Parcels), g.Sum(c => c.WeightKg)));
        }
        return result;
    }

    public async Task<List<DriverWorkloadRow>> DriverWorkloadAsync()
    {
        var drivers = await _drivers.ListAsync(null);
        var depots = (await _depots.ListAsync(null)).ToDictionary(d => d.Id);
        var rows = new List<DriverWorkloadRow>();
        foreach (var d in drivers.Where(d => d.IsActive))
            rows.Add(new DriverWorkloadRow(d.Id, d.Name, depots[d.DepotId].Name, await _consignments.CountOpenForDriverAsync(d.Id), d.MaxStops));
        return rows.OrderByDescending(r => r.AssignedStops).ToList();
    }

    public static double Pct(int part, int whole) => whole == 0 ? 0 : Math.Round(100.0 * part / whole, 1);
}
