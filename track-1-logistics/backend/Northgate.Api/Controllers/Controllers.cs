using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Northgate.Api.Dtos;
using Northgate.Api.Infrastructure;
using Northgate.Api.Services;

namespace Northgate.Api.Controllers;

[ApiController]
[Route("api/depots")]
public class DepotsController : ControllerBase
{
    private readonly DepotService _service;
    private readonly IValidator<DepotWriteDto> _validator;
    private readonly ConsignmentService _consignments;
    private readonly DriverService _drivers;

    public DepotsController(DepotService service, IValidator<DepotWriteDto> validator, ConsignmentService consignments, DriverService drivers)
    {
        _service = service; _validator = validator; _consignments = consignments; _drivers = drivers;
    }

    [HttpGet] public async Task<ActionResult<List<DepotDto>>> List([FromQuery] string? region) => await _service.ListAsync(region);
    [HttpGet("{id:int}")] public async Task<ActionResult<DepotDto>> Get(int id) => await _service.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<DepotDto>> Create(DepotWriteDto dto)
    {
        await _validator.ValidateAndThrowAsync(dto);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepotDto>> Update(int id, DepotWriteDto dto)
    {
        await _validator.ValidateAndThrowAsync(dto);
        return await _service.UpdateAsync(id, dto);
    }

    [HttpGet("{id:int}/drivers")] public async Task<ActionResult<List<DriverDto>>> Drivers(int id) => await _drivers.ListAsync(id);

    [HttpGet("{id:int}/consignments")]
    public async Task<ActionResult<PagedResult<ConsignmentListItemDto>>> Consignments(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) =>
        await _consignments.SearchAsync(new ConsignmentFilter(DepotId: id, Page: page, PageSize: pageSize));
}

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _service;
    private readonly IValidator<CustomerWriteDto> _validator;

    public CustomersController(CustomerService service, IValidator<CustomerWriteDto> validator)
    {
        _service = service; _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CustomerDto>>> Search([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) =>
        await _service.SearchAsync(search, page, pageSize);

    [HttpGet("{id:int}")] public async Task<ActionResult<CustomerDto>> Get(int id) => await _service.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CustomerWriteDto dto)
    {
        await _validator.ValidateAndThrowAsync(dto);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerDto>> Update(int id, CustomerWriteDto dto)
    {
        await _validator.ValidateAndThrowAsync(dto);
        return await _service.UpdateAsync(id, dto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _service.DeactivateAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/consignments")]
    public async Task<ActionResult<List<ConsignmentListItemDto>>> Consignments(int id) => await _service.ListConsignmentsAsync(id);
}

[ApiController]
[Route("api/consignments")]
public class ConsignmentsController : ControllerBase
{
    private readonly ConsignmentService _service;
    private readonly IValidator<CreateConsignmentDto> _createValidator;
    private readonly IValidator<UpdateConsignmentDto> _updateValidator;
    private readonly IValidator<CreateNoteDto> _noteValidator;
    private readonly IValidator<CancelConsignmentDto> _cancelValidator;

    public ConsignmentsController(ConsignmentService service, IValidator<CreateConsignmentDto> createValidator,
        IValidator<UpdateConsignmentDto> updateValidator, IValidator<CreateNoteDto> noteValidator, IValidator<CancelConsignmentDto> cancelValidator)
    {
        _service = service; _createValidator = createValidator; _updateValidator = updateValidator;
        _noteValidator = noteValidator; _cancelValidator = cancelValidator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ConsignmentListItemDto>>> Search([FromQuery] string? status, [FromQuery] int? depotId,
        [FromQuery] int? customerId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? search,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 50) =>
        await _service.SearchAsync(new ConsignmentFilter(status, depotId, customerId, from, to, search, page, pageSize));

    [HttpGet("{id:int}")] public async Task<ActionResult<ConsignmentDetailDto>> Get(int id) => await _service.GetAsync(id);

    [HttpGet("by-reference/{reference}")]
    public async Task<ActionResult<ConsignmentDetailDto>> GetByReference(string reference) => await _service.GetByReferenceAsync(reference);

    [HttpPost]
    public async Task<ActionResult<ConsignmentDetailDto>> Create(CreateConsignmentDto dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ConsignmentDetailDto>> Update(int id, UpdateConsignmentDto dto)
    {
        await _updateValidator.ValidateAndThrowAsync(dto);
        return await _service.UpdateAsync(id, dto);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ConsignmentDetailDto>> Cancel(int id, CancelConsignmentDto dto)
    {
        await _cancelValidator.ValidateAndThrowAsync(dto);
        return await _service.CancelAsync(id, dto.Reason);
    }

    [HttpGet("{id:int}/notes")] public async Task<ActionResult<List<NoteDto>>> Notes(int id) => await _service.ListNotesAsync(id);

    [HttpPost("{id:int}/notes")]
    public async Task<ActionResult<NoteDto>> AddNote(int id, CreateNoteDto dto)
    {
        await _noteValidator.ValidateAndThrowAsync(dto);
        return Ok(await _service.AddNoteAsync(id, dto));
    }

    [HttpGet("{id:int}/events")] public async Task<ActionResult<List<EventDto>>> Events(int id) => await _service.ListEventsAsync(id);
}

[ApiController]
[Route("api/drivers")]
public class DriversController : ControllerBase
{
    private readonly DriverService _service;
    private readonly IValidator<DriverWriteDto> _validator;

    public DriversController(DriverService service, IValidator<DriverWriteDto> validator)
    {
        _service = service; _validator = validator;
    }

    [HttpGet] public async Task<ActionResult<List<DriverDto>>> List([FromQuery] int? depotId) => await _service.ListAsync(depotId);
    [HttpGet("{id:int}")] public async Task<ActionResult<DriverDto>> Get(int id) => await _service.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<DriverDto>> Create(DriverWriteDto dto)
    {
        await _validator.ValidateAndThrowAsync(dto);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DriverDto>> Update(int id, DriverWriteDto dto)
    {
        await _validator.ValidateAndThrowAsync(dto);
        return await _service.UpdateAsync(id, dto);
    }

    [HttpPost("{id:int}/assign")]
    public async Task<ActionResult<DriverDto>> Assign(int id, AssignConsignmentDto dto) => await _service.AssignAsync(id, dto.ConsignmentId);

    [HttpGet("{id:int}/manifest")]
    public async Task<ActionResult<List<ConsignmentListItemDto>>> Manifest(int id) => await _service.ManifestAsync(id);
}

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _service;
    public ReportsController(ReportService service) => _service = service;

    [HttpGet("dispatch-on-time")]
    public async Task<ActionResult<DispatchOnTimeReport>> DispatchOnTime([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int? depotId)
    {
        if (to < from) throw new BusinessRuleException("'to' must not be before 'from'.");
        return await _service.DispatchOnTimeAsync(from, to, depotId);
    }

    [HttpGet("depot-volumes")]
    public async Task<ActionResult<List<DepotVolumeRow>>> DepotVolumes([FromQuery] DateTime from, [FromQuery] DateTime to) =>
        await _service.DepotVolumesAsync(from, to);

    [HttpGet("customer-volumes")]
    public async Task<ActionResult<List<CustomerVolumeRow>>> CustomerVolumes([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 10) =>
        await _service.CustomerVolumesAsync(from, to, top);

    [HttpGet("driver-workload")]
    public async Task<ActionResult<List<DriverWorkloadRow>>> DriverWorkload() => await _service.DriverWorkloadAsync();
}

[ApiController]
[Route("api/imports")]
public class ImportsController : ControllerBase
{
    private readonly ManifestImportService _service;
    public ImportsController(ManifestImportService service) => _service = service;

    [HttpPost("manifest")]
    public async Task<ActionResult<ImportResultDto>> Manifest(IFormFile file)
    {
        if (file == null || file.Length == 0) throw new BusinessRuleException("Choose a CSV file to upload.");
        await using var stream = file.OpenReadStream();
        return await _service.ImportAsync(file.FileName, stream);
    }

    [HttpGet] public async Task<ActionResult<List<ImportBatchDto>>> List() => await _service.ListBatchesAsync();
    [HttpGet("{id:int}")] public async Task<ActionResult<ImportBatchDto>> Get(int id) => await _service.GetBatchAsync(id);
}

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;
    public UsersController(UserService service) => _service = service;

    [HttpGet] public async Task<ActionResult<List<UserDto>>> List() => await _service.ListAsync();
    [HttpGet("{id:int}")] public async Task<ActionResult<UserDto>> Get(int id) => await _service.GetAsync(id);
    [HttpPost("login")] public async Task<ActionResult<LoginResultDto>> Login(LoginRequestDto dto) => await _service.LoginAsync(dto);
}

[ApiController]
[Route("api")]
public class MetaController : ControllerBase
{
    [HttpGet("health")] public IActionResult Health() => Ok(new { status = "ok" });
    [HttpGet("service-levels")] public IActionResult ServiceLevels() => Ok(Models.ServiceLevels.All);
    [HttpGet("statuses")] public IActionResult Statuses() => Ok(Models.ConsignmentStatus.All);
}
