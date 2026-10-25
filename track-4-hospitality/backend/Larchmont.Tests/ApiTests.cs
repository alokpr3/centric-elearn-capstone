using System.Net;
using System.Net.Http.Json;
using Larchmont.Api.Dtos;

namespace Larchmont.Tests;

public class ApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public ApiTests(ApiFactory factory)
    {
        _factory = factory;
        factory.SeedBasics();
    }

    [Fact]
    public async Task Hotels_List_ReturnsSeededHotel()
    {
        var client = _factory.CreateClient();
        var hotels = await client.GetFromJsonAsync<List<HotelDto>>("/api/hotels");
        Assert.NotNull(hotels);
        Assert.Contains(hotels!, h => h.Code == "EDI");
    }

    [Fact]
    public async Task RoomTypes_List_ReturnsOk()
    {
        var response = await _factory.CreateClient().GetAsync("/api/room-types");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Booking_UnknownId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().GetAsync("/api/bookings/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Guest_WithInvalidEmail_ReturnsBadRequest()
    {
        var bad = new GuestRequest("Ada", "Lovelace", "not-an-email", "+44 7700 900001", "1 High Street", "Leeds", "LS1 1AA", "GB", "North");
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/guests", bad);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
