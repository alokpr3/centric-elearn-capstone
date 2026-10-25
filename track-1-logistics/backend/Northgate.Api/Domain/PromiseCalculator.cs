using Northgate.Api.Models;

namespace Northgate.Api.Domain;

public static class PromiseCalculator
{
    public static DateTime Calculate(DateTime bookedAt, string serviceLevel)
    {
        var day = bookedAt.Date;
        return serviceLevel switch
        {
            ServiceLevels.NextDay => day.AddDays(1).AddHours(17),
            ServiceLevels.ExpressAm => day.AddDays(1).AddHours(9),
            ServiceLevels.Economy => day.AddDays(3).AddHours(17),
            ServiceLevels.Pallet48 => day.AddDays(2).AddHours(17),
            ServiceLevels.SameDay => bookedAt.Hour < 12 ? day.AddHours(20) : day.AddDays(1).AddHours(12),
            _ => throw new ArgumentOutOfRangeException(nameof(serviceLevel), serviceLevel, "Unknown service level")
        };
    }
}
