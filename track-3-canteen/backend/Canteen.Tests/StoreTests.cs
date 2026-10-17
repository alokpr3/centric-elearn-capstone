using Canteen.Api.Data;
using Canteen.Api.Models;
using Xunit;

namespace Canteen.Tests;

public class StoreTests
{
    [Fact]
    public void GetMenu_ReturnsItems()
    {
        var store = new InMemoryStore();

        Assert.NotEmpty(store.GetMenu());
    }

    [Fact]
    public void PlaceOrder_SingleItem_TotalIsItemPrice()
    {
        var store = new InMemoryStore();

        var order = store.PlaceOrder("Asha", new[] { new OrderLine(1, 1) });

        Assert.Equal(60m, order.Total);
    }

    [Fact]
    public void PlaceOrder_MultipleQuantity_TotalUsesQuantity()
    {
        var store = new InMemoryStore();

        var order = store.PlaceOrder("Asha", new[] { new OrderLine(1, 3), new OrderLine(3, 2) });

        Assert.Equal(230m, order.Total);
    }

    [Fact]
    public void PlaceOrder_UnavailableItem_IsRejected()
    {
        var store = new InMemoryStore();

        Assert.Throws<ArgumentException>(() => store.PlaceOrder("Asha", new[] { new OrderLine(5, 1) }));
    }

    [Fact]
    public void PlaceOrder_NoItems_IsRejected()
    {
        var store = new InMemoryStore();

        Assert.Throws<ArgumentException>(() => store.PlaceOrder("Asha", Array.Empty<OrderLine>()));
    }
}
