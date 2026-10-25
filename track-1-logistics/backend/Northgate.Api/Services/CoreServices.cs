using System.Security.Cryptography;
using Northgate.Api.Dtos;
using Northgate.Api.Infrastructure;
using Northgate.Api.Mappers;
using Northgate.Api.Models;
using Northgate.Api.Repositories;

namespace Northgate.Api.Services;

public class DepotService
{
    private readonly IDepotRepository _depots;
    public DepotService(IDepotRepository depots) => _depots = depots;

    public async Task<List<DepotDto>> ListAsync(string? region) =>
        (await _depots.ListAsync(region?.ToUpperInvariant())).Select(DepotMapper.ToDto).ToList();

    public async Task<DepotDto> GetAsync(int id) =>
        DepotMapper.ToDto(await _depots.GetAsync(id) ?? throw new NotFoundException($"Depot {id} not found."));

    public async Task<DepotDto> CreateAsync(DepotWriteDto dto)
    {
        if (await _depots.GetByCodeAsync(dto.Code.Trim().ToUpperInvariant()) != null)
            throw new ConflictException($"Depot code {dto.Code} already exists.");
        var depot = new Depot { CreatedAt = DateTime.UtcNow };
        DepotMapper.Apply(depot, dto);
        await _depots.AddAsync(depot);
        await _depots.SaveAsync();
        return DepotMapper.ToDto(depot);
    }

    public async Task<DepotDto> UpdateAsync(int id, DepotWriteDto dto)
    {
        var depot = await _depots.GetAsync(id) ?? throw new NotFoundException($"Depot {id} not found.");
        DepotMapper.Apply(depot, dto);
        await _depots.SaveAsync();
        return DepotMapper.ToDto(depot);
    }
}

public class CustomerService
{
    private readonly ICustomerRepository _customers;
    private readonly IConsignmentRepository _consignments;
    private readonly IDepotRepository _depots;
    private readonly IDriverRepository _drivers;

    public CustomerService(ICustomerRepository customers, IConsignmentRepository consignments, IDepotRepository depots, IDriverRepository drivers)
    {
        _customers = customers; _consignments = consignments; _depots = depots; _drivers = drivers;
    }

    public async Task<PagedResult<CustomerDto>> SearchAsync(string? search, int page, int pageSize)
    {
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 200);
        var (items, total) = await _customers.SearchAsync(search, page, pageSize);
        return new PagedResult<CustomerDto>(items.Select(CustomerMapper.ToDto).ToList(), total, page, pageSize);
    }

    public async Task<CustomerDto> GetAsync(int id) =>
        CustomerMapper.ToDto(await _customers.GetAsync(id) ?? throw new NotFoundException($"Customer {id} not found."));

    public async Task<CustomerDto> CreateAsync(CustomerWriteDto dto)
    {
        if (await _customers.GetByAccountNoAsync(dto.AccountNo.Trim().ToUpperInvariant()) != null)
            throw new ConflictException($"Account {dto.AccountNo} already exists.");
        var customer = new Customer { CreatedAt = DateTime.UtcNow, IsActive = true };
        CustomerMapper.Apply(customer, dto);
        await _customers.AddAsync(customer);
        await _customers.SaveAsync();
        return CustomerMapper.ToDto(customer);
    }

    public async Task<CustomerDto> UpdateAsync(int id, CustomerWriteDto dto)
    {
        var customer = await _customers.GetAsync(id) ?? throw new NotFoundException($"Customer {id} not found.");
        CustomerMapper.Apply(customer, dto);
        await _customers.SaveAsync();
        return CustomerMapper.ToDto(customer);
    }

    public async Task DeactivateAsync(int id)
    {
        var customer = await _customers.GetAsync(id) ?? throw new NotFoundException($"Customer {id} not found.");
        customer.IsActive = false;
        await _customers.SaveAsync();
    }

    public async Task<List<ConsignmentListItemDto>> ListConsignmentsAsync(int id)
    {
        var customer = await _customers.GetAsync(id) ?? throw new NotFoundException($"Customer {id} not found.");
        var list = await _consignments.ListByCustomerAsync(id);
        var result = new List<ConsignmentListItemDto>();
        foreach (var c in list)
        {
            var origin = await _depots.GetAsync(c.OriginDepotId);
            var dest = await _depots.GetAsync(c.DestDepotId);
            var driver = c.DriverId.HasValue ? await _drivers.GetAsync(c.DriverId.Value) : null;
            result.Add(ConsignmentMapper.ToListItem(c, customer.Name, origin?.Name ?? "", dest?.Name ?? "", driver?.Name));
        }
        return result;
    }
}

public class UserService
{
    private readonly IUserRepository _users;
    public UserService(IUserRepository users) => _users = users;

    public async Task<List<UserDto>> ListAsync() => (await _users.ListAsync()).Select(UserMapper.ToDto).ToList();

    public async Task<UserDto> GetAsync(int id) =>
        UserMapper.ToDto(await _users.GetAsync(id) ?? throw new NotFoundException($"User {id} not found."));

    public async Task<LoginResultDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _users.GetByUsernameAsync(request.Username);
        if (user == null || !user.IsActive || !Verify(request.Password, user.PasswordHash))
            throw new BusinessRuleException("Invalid username or password.");
        user.LastLoginAt = DateTime.UtcNow;
        await _users.SaveAsync();
        return new LoginResultDto(user.Id, user.Username, user.Role, user.RegionCode);
    }

    public static bool Verify(string password, string stored)
    {
        var parts = stored.Split('$');
        if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256") return false;
        var iterations = int.Parse(parts[1]);
        var salt = Convert.FromBase64String(parts[2]);
        var expected = Convert.FromBase64String(parts[3]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}

public class DriverService
{
    private readonly IDriverRepository _drivers;
    private readonly IDepotRepository _depots;
    private readonly IConsignmentRepository _consignments;

    public DriverService(IDriverRepository drivers, IDepotRepository depots, IConsignmentRepository consignments)
    {
        _drivers = drivers; _depots = depots; _consignments = consignments;
    }

    public async Task<List<DriverDto>> ListAsync(int? depotId)
    {
        var drivers = await _drivers.ListAsync(depotId);
        var result = new List<DriverDto>();
        foreach (var d in drivers)
        {
            var depot = await _depots.GetAsync(d.DepotId);
            result.Add(DriverMapper.ToDto(d, depot?.Name ?? "", await _consignments.CountOpenForDriverAsync(d.Id)));
        }
        return result;
    }

    public async Task<DriverDto> GetAsync(int id)
    {
        var driver = await _drivers.GetAsync(id) ?? throw new NotFoundException($"Driver {id} not found.");
        var depot = await _depots.GetAsync(driver.DepotId);
        return DriverMapper.ToDto(driver, depot?.Name ?? "", await _consignments.CountOpenForDriverAsync(id));
    }

    public async Task<DriverDto> CreateAsync(DriverWriteDto dto)
    {
        var depot = await _depots.GetAsync(dto.DepotId) ?? throw new NotFoundException($"Depot {dto.DepotId} not found.");
        var driver = new Driver { CreatedAt = DateTime.UtcNow, IsActive = true };
        DriverMapper.Apply(driver, dto);
        await _drivers.AddAsync(driver);
        await _drivers.SaveAsync();
        return DriverMapper.ToDto(driver, depot.Name, 0);
    }

    public async Task<DriverDto> UpdateAsync(int id, DriverWriteDto dto)
    {
        var driver = await _drivers.GetAsync(id) ?? throw new NotFoundException($"Driver {id} not found.");
        var depot = await _depots.GetAsync(dto.DepotId) ?? throw new NotFoundException($"Depot {dto.DepotId} not found.");
        DriverMapper.Apply(driver, dto);
        await _drivers.SaveAsync();
        return DriverMapper.ToDto(driver, depot.Name, await _consignments.CountOpenForDriverAsync(id));
    }

    public async Task<DriverDto> AssignAsync(int driverId, int consignmentId)
    {
        var driver = await _drivers.GetAsync(driverId) ?? throw new NotFoundException($"Driver {driverId} not found.");
        var consignment = await _consignments.GetAsync(consignmentId) ?? throw new NotFoundException($"Consignment {consignmentId} not found.");
        if (!driver.IsActive) throw new BusinessRuleException("Driver is not active.");
        if (consignment.Status is ConsignmentStatus.Delivered or ConsignmentStatus.Cancelled)
            throw new BusinessRuleException($"A {consignment.Status} consignment cannot be assigned.");
        if (consignment.DriverId.HasValue) throw new ConflictException("Consignment already has a driver.");
        var load = await _consignments.CountOpenForDriverAsync(driver.Id);
        if (load >= driver.MaxStops) throw new ConflictException("Driver is already at the stop limit.");

        consignment.DriverId = driver.Id;
        consignment.UpdatedAt = DateTime.UtcNow;
        await _consignments.SaveAsync();

        var depot = await _depots.GetAsync(driver.DepotId);
        return DriverMapper.ToDto(driver, depot?.Name ?? "", load + 1);
    }

    public async Task<List<ConsignmentListItemDto>> ManifestAsync(int driverId)
    {
        var driver = await _drivers.GetAsync(driverId) ?? throw new NotFoundException($"Driver {driverId} not found.");
        var list = await _consignments.ListByDriverAsync(driverId);
        return list.Where(c => c.Status is ConsignmentStatus.OutForDelivery or ConsignmentStatus.Booked or ConsignmentStatus.Collected or ConsignmentStatus.AtDepot or ConsignmentStatus.InTransit)
            .Select(c => ConsignmentMapper.ToListItem(c, "", "", "", driver.Name)).ToList();
    }
}
