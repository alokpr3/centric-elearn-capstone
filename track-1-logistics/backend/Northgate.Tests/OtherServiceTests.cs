using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using Moq;
using Northgate.Api.Dtos;
using Northgate.Api.Infrastructure;
using Northgate.Api.Models;
using Northgate.Api.Repositories;
using Northgate.Api.Services;
using Northgate.Api.Validators;

namespace Northgate.Tests;

public class DriverServiceTests
{
    private readonly Mock<IDriverRepository> _drivers = new();
    private readonly Mock<IDepotRepository> _depots = new();
    private readonly Mock<IConsignmentRepository> _consignments = new();

    [Fact]
    public async Task Assign_DriverAtStopLimit_IsRefused()
    {
        _drivers.Setup(d => d.GetAsync(1)).ReturnsAsync(new Driver { Id = 1, MaxStops = 5, IsActive = true });
        _consignments.Setup(c => c.CountOpenForDriverAsync(1)).ReturnsAsync(5);
        _consignments.Setup(c => c.GetAsync(10)).ReturnsAsync(new Consignment { Id = 10, Status = ConsignmentStatus.Booked });
        var service = new DriverService(_drivers.Object, _depots.Object, _consignments.Object);

        await Assert.ThrowsAsync<ConflictException>(() => service.AssignAsync(1, 10));
    }

    [Fact]
    public async Task Assign_DriverWithSpace_IncrementsAssignedStops()
    {
        var driver = new Driver { Id = 1, DepotId = 1, MaxStops = 5, IsActive = true };
        _consignments.Setup(c => c.CountOpenForDriverAsync(1)).ReturnsAsync(2);
        _drivers.Setup(d => d.GetAsync(1)).ReturnsAsync(driver);
        _depots.Setup(d => d.GetAsync(1)).ReturnsAsync(new Depot { Id = 1, Name = "Leeds" });
        _consignments.Setup(c => c.GetAsync(10)).ReturnsAsync(new Consignment { Id = 10, Status = ConsignmentStatus.Booked });
        var service = new DriverService(_drivers.Object, _depots.Object, _consignments.Object);

        var result = await service.AssignAsync(1, 10);

        Assert.Equal(3, result.AssignedStops);
    }
}

public class ReportServiceTests
{
    [Fact]
    public void Pct_RoundsToOneDecimal() => Assert.Equal(66.7, ReportService.Pct(2, 3));

    [Fact]
    public async Task DispatchOnTime_AllDeliveredBeforePromise_IsOneHundredPercent()
    {
        var promised = new DateTime(2026, 9, 10, 17, 0, 0);
        var consignments = new Mock<IConsignmentRepository>();
        consignments.Setup(c => c.ListPromisedBetweenAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(new List<Consignment>
        {
            new() { Id = 1, OriginDepotId = 1, Status = ConsignmentStatus.Delivered, PromisedBy = promised, DeliveredAt = promised.AddHours(-2) },
            new() { Id = 2, OriginDepotId = 1, Status = ConsignmentStatus.Delivered, PromisedBy = promised, DeliveredAt = promised.AddHours(-5) }
        });
        var depots = new Mock<IDepotRepository>();
        depots.Setup(d => d.ListAsync(null)).ReturnsAsync(new List<Depot> { new() { Id = 1, Code = "LDS", Name = "Leeds" } });
        var service = new ReportService(consignments.Object, depots.Object, Mock.Of<ICustomerRepository>(), Mock.Of<IDriverRepository>());

        var report = await service.DispatchOnTimeAsync(new DateTime(2026, 9, 1), new DateTime(2026, 9, 30), null);

        Assert.Equal(100.0, report.OnTimePct);
    }
}

public class ValidatorTests
{
    private static CreateConsignmentDto Valid() =>
        new(1, 1, 2, ServiceLevels.Economy, 2, 10, "Alan Turner", "07412 556120", "14 Mill Road", "LS1 4DL");

    [Fact]
    public void CreateConsignment_ZeroParcels_IsInvalid()
    {
        var result = new CreateConsignmentValidator().Validate(Valid() with { Parcels = 0 });
        Assert.False(result.IsValid);
    }
}

public class UserServiceTests
{
    private static string Hash(string password)
    {
        var salt = Encoding.UTF8.GetBytes("0123456789abcdef");
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 1000, HashAlgorithmName.SHA256, 32);
        return $"PBKDF2-SHA256$1000${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue() => Assert.True(UserService.Verify("Northgate!2026", Hash("Northgate!2026")));
}

public class ManifestImportServiceTests
{
    private static async Task<ImportResultDto> Run(string csv, Mock<IConsignmentRepository> consignments)
    {
        var customers = new Mock<ICustomerRepository>();
        customers.Setup(c => c.GetByAccountNoAsync("NGC01000")).ReturnsAsync(new Customer { Id = 1, AccountNo = "NGC01000", IsActive = true });
        var depots = new Mock<IDepotRepository>();
        depots.Setup(d => d.GetByCodeAsync("LDS")).ReturnsAsync(new Depot { Id = 4, Code = "LDS", RegionCode = "NE" });
        depots.Setup(d => d.GetByCodeAsync("MAN")).ReturnsAsync(new Depot { Id = 1, Code = "MAN", RegionCode = "NW" });
        consignments.Setup(c => c.NextReferenceAsync()).ReturnsAsync("NGF40300001");
        var service = new ManifestImportService(consignments.Object, customers.Object, depots.Object, Mock.Of<IImportBatchRepository>());
        return await service.ImportAsync("test.csv", new MemoryStream(Encoding.UTF8.GetBytes(csv)));
    }

    private const string Header = "reference,account_no,origin_depot,dest_depot,service_level,parcels,weight_kg,recipient_name,recipient_phone,delivery_address,delivery_postcode,booked_date,notes\n";

    [Fact]
    public async Task Import_ValidRow_IsImported()
    {
        var result = await Run(Header + ",NGC01000,LDS,MAN,NEXT_DAY,2,14.5,Alan Turner,07412,\"9 Mill Road, Salford\",M5 4QT,2026-10-01,\n", new Mock<IConsignmentRepository>());
        Assert.Equal(1, result.ImportedRows);
    }

    [Fact]
    public async Task Import_UnknownDepot_IsSkipped()
    {
        var result = await Run(Header + ",NGC01000,XXX,MAN,NEXT_DAY,2,14.5,Alan Turner,07412,9 Mill Road,M5 4QT,2026-10-01,\n", new Mock<IConsignmentRepository>());
        Assert.Equal(1, result.SkippedRows);
    }
}

public class ApiSmokeTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    public ApiSmokeTests(ApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk() => Assert.Equal(System.Net.HttpStatusCode.OK, (await _client.GetAsync("/api/health")).StatusCode);

    [Fact]
    public async Task Depots_ReturnsAllEighteen()
    {
        var depots = await _client.GetFromJsonAsync<List<DepotDto>>("/api/depots");
        Assert.Equal(18, depots!.Count);
    }

    [Fact]
    public async Task Consignments_FirstPageHasFiftyRows()
    {
        var page = await _client.GetFromJsonAsync<PagedResult<ConsignmentListItemDto>>("/api/consignments");
        Assert.Equal(50, page!.Items.Count);
    }

    [Fact]
    public async Task UnknownConsignment_ReturnsNotFoundProblem()
    {
        var response = await _client.GetAsync("/api/consignments/999999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
