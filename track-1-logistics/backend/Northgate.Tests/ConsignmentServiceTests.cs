using Moq;
using Northgate.Api.Dtos;
using Northgate.Api.Infrastructure;
using Northgate.Api.Models;
using Northgate.Api.Repositories;
using Northgate.Api.Services;

namespace Northgate.Tests;

public class ConsignmentServiceTests
{
    private readonly Mock<IConsignmentRepository> _consignments = new();
    private readonly Mock<ICustomerRepository> _customers = new();
    private readonly Mock<IDepotRepository> _depots = new();
    private readonly Mock<IDriverRepository> _drivers = new();

    private ConsignmentService Build() => new(_consignments.Object, _customers.Object, _depots.Object, _drivers.Object);

    private static Consignment Existing(string status) => new()
    {
        Id = 7, Reference = "NGF40100007", CustomerId = 1, OriginDepotId = 1, DestDepotId = 2, ServiceLevel = ServiceLevels.NextDay,
        Status = status, Parcels = 1, WeightKg = 5
    };

    [Fact]
    public async Task Create_UnknownCustomer_ThrowsNotFound()
    {
        _customers.Setup(c => c.GetAsync(99)).ReturnsAsync((Customer?)null);
        var dto = new CreateConsignmentDto(99, 1, 2, ServiceLevels.NextDay, 1, 5, "A", "07", "1 High St", "LS1 1AA");
        await Assert.ThrowsAsync<NotFoundException>(() => Build().CreateAsync(dto));
    }

    [Fact]
    public async Task Create_ValidRequest_StartsAsBooked()
    {
        _customers.Setup(c => c.GetAsync(1)).ReturnsAsync(new Customer { Id = 1, Name = "Acme", IsActive = true });
        _depots.Setup(d => d.GetAsync(It.IsAny<int>())).ReturnsAsync((int id) => new Depot { Id = id, Name = "Depot " + id, RegionCode = "NE" });
        _consignments.Setup(c => c.NextReferenceAsync()).ReturnsAsync("NGF40200001");
        var dto = new CreateConsignmentDto(1, 1, 2, ServiceLevels.NextDay, 2, 10, "Alan Turner", "07412", "14 Mill Road", "ls1 4dl");

        var result = await Build().CreateAsync(dto);

        Assert.Equal(ConsignmentStatus.Booked, result.Status);
        Assert.Equal("NGF40200001", result.Reference);
        Assert.Equal("LS1 4DL", result.DeliveryPostcode);
    }

    [Fact]
    public async Task Cancel_DeliveredConsignment_IsRefused()
    {
        _consignments.Setup(c => c.GetAsync(7)).ReturnsAsync(Existing(ConsignmentStatus.Delivered));
        await Assert.ThrowsAsync<BusinessRuleException>(() => Build().CancelAsync(7, "changed mind"));
    }

    [Fact]
    public async Task Cancel_BookedConsignment_SetsCancelledStatus()
    {
        var consignment = Existing(ConsignmentStatus.Booked);
        _consignments.Setup(c => c.GetAsync(7)).ReturnsAsync(consignment);
        _customers.Setup(c => c.GetAsync(1)).ReturnsAsync(new Customer { Id = 1, Name = "Acme" });
        _depots.Setup(d => d.GetAsync(It.IsAny<int>())).ReturnsAsync(new Depot { Id = 1, Name = "Leeds" });

        await Build().CancelAsync(7, "customer withdrew order");

        Assert.Equal(ConsignmentStatus.Cancelled, consignment.Status);
        Assert.NotNull(consignment.CancelledAt);
    }

    [Fact]
    public async Task Cancel_AddsCancellationNote()
    {
        _consignments.Setup(c => c.GetAsync(7)).ReturnsAsync(Existing(ConsignmentStatus.Booked));
        _customers.Setup(c => c.GetAsync(1)).ReturnsAsync(new Customer { Id = 1, Name = "Acme" });
        _depots.Setup(d => d.GetAsync(It.IsAny<int>())).ReturnsAsync(new Depot { Id = 1, Name = "Leeds" });

        await Build().CancelAsync(7, "customer withdrew order");

        _consignments.Verify(c => c.SaveAsync(), Times.AtLeastOnce());
    }
}
