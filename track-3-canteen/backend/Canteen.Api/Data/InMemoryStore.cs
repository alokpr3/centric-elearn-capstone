using Canteen.Api.Models;

namespace Canteen.Api.Data;

public class InMemoryStore
{
    private readonly List<MenuItem> _menu = new()
    {
        new MenuItem(1, "Veg Sandwich", 60m, true),
        new MenuItem(2, "Masala Dosa", 80m, true),
        new MenuItem(3, "Filter Coffee", 25m, true),
        new MenuItem(4, "Chicken Biryani", 150m, true),
        new MenuItem(5, "Fruit Salad", 70m, false),
    };

    private readonly List<Order> _orders = new();

    public IReadOnlyList<MenuItem> GetMenu() => _menu;

    public IReadOnlyList<Order> GetOrders() => _orders;

    public Order PlaceOrder(string employeeName, IEnumerable<OrderLine> lines)
    {
        var lineList = lines.ToList();
        if (lineList.Count == 0)
        {
            throw new ArgumentException("An order needs at least one item.");
        }

        foreach (var line in lineList)
        {
            var item = _menu.FirstOrDefault(m => m.Id == line.MenuItemId)
                ?? throw new ArgumentException($"Menu item {line.MenuItemId} does not exist.");
            if (!item.Available)
            {
                throw new ArgumentException($"{item.Name} is not available today.");
            }

            if (line.Quantity < 0)
            {
                throw new ArgumentException("Quantity must be a positive number.");
            }
        }

        var order = new Order(_orders.Count + 1, employeeName, lineList, CalculateTotal(lineList), DateTime.Now);
        _orders.Add(order);
        return order;
    }

    public decimal CalculateTotal(IEnumerable<OrderLine> lines)
    {
        decimal total = 0;
        foreach (var line in lines)
        {
            var item = _menu.First(m => m.Id == line.MenuItemId);
            total += item.Price;
        }

        return total;
    }
}
