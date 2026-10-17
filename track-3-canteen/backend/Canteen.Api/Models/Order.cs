namespace Canteen.Api.Models;

public record OrderLine(int MenuItemId, int Quantity);

public record Order(int Id, string EmployeeName, List<OrderLine> Lines, decimal Total, DateTime PlacedAt);

public record PlaceOrderRequest(string EmployeeName, List<OrderLine> Items);
