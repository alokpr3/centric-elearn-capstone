using Larchmont.Api.Dtos;
using Larchmont.Api.Repositories;
using Larchmont.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Larchmont.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookings;
    public BookingsController(IBookingService bookings) => _bookings = bookings;

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookingDto>>> Search([FromQuery] int? hotelId, [FromQuery] int? guestId,
        [FromQuery] string? status, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 50) =>
        await _bookings.SearchAsync(new BookingFilter(hotelId, guestId, status, from, to, page, pageSize));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> Get(int id) => await _bookings.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(BookingRequest request)
    {
        var created = await _bookings.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingDto>> Update(int id, BookingUpdateRequest request) => await _bookings.UpdateAsync(id, request);

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<CancelResultDto>> Cancel(int id) => await _bookings.CancelAsync(id);

    [HttpPost("{id:int}/check-in")]
    public async Task<ActionResult<BookingDto>> CheckIn(int id) => await _bookings.CheckInAsync(id);

    [HttpPost("{id:int}/check-out")]
    public async Task<ActionResult<BookingDto>> CheckOut(int id) => await _bookings.CheckOutAsync(id);

    [HttpPost("{id:int}/assign-room")]
    public async Task<ActionResult<BookingDto>> AssignRoom(int id, AssignRoomRequest request) => await _bookings.AssignRoomAsync(id, request);

    [HttpGet("{id:int}/notes")]
    public async Task<ActionResult<List<BookingNoteDto>>> Notes(int id) => await _bookings.ListNotesAsync(id);

    [HttpPost("{id:int}/notes")]
    public async Task<ActionResult<BookingNoteDto>> AddNote(int id, BookingNoteRequest request) => await _bookings.AddNoteAsync(id, request);
}

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    [HttpGet("occupancy")]
    public async Task<ActionResult<OccupancyReportDto>> Occupancy([FromQuery] int? hotelId, [FromQuery] DateOnly from, [FromQuery] DateOnly to) =>
        await _reports.GetOccupancyAsync(hotelId, from, to);

    [HttpGet("arrivals")]
    public async Task<ActionResult<List<ArrivalDto>>> Arrivals([FromQuery] DateOnly date, [FromQuery] int? hotelId) =>
        await _reports.GetArrivalsAsync(date, hotelId);

    [HttpGet("revenue")]
    public async Task<ActionResult<List<RevenueRowDto>>> Revenue([FromQuery] DateOnly from, [FromQuery] DateOnly to) =>
        await _reports.GetRevenueAsync(from, to);
}

[ApiController]
[Route("api/imports")]
public class ImportsController : ControllerBase
{
    private readonly IChannelImportService _import;
    public ImportsController(IChannelImportService import) => _import = import;

    [HttpPost("channel-bookings")]
    public async Task<ActionResult<ImportResultDto>> ChannelBookings(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        return await _import.ImportAsync(await reader.ReadToEndAsync());
    }
}
