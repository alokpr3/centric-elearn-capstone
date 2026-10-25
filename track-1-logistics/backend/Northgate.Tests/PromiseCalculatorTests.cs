using Northgate.Api.Domain;
using Northgate.Api.Models;

namespace Northgate.Tests;

public class PromiseCalculatorTests
{
    [Fact]
    public void NextDay_IsPromisedForFivePmTheFollowingDay()
    {
        var promised = PromiseCalculator.Calculate(new DateTime(2026, 9, 10, 14, 30, 0), ServiceLevels.NextDay);
        Assert.Equal(new DateTime(2026, 9, 11, 17, 0, 0), promised);
    }

    [Fact]
    public void SameDay_BookedAfterNoon_RollsToNextMidday()
    {
        var promised = PromiseCalculator.Calculate(new DateTime(2026, 9, 10, 13, 0, 0), ServiceLevels.SameDay);
        Assert.Equal(new DateTime(2026, 9, 11, 12, 0, 0), promised);
    }

    [Fact]
    public void NextDay_PromisedDayComesAfterBookedDay()
    {
        var booked = DateTime.Now;
        var promised = PromiseCalculator.Calculate(booked, ServiceLevels.NextDay);
        Assert.True(promised.Day > booked.Day);
    }
}
