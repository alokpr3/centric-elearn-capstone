using Canteen.Api.Data;
using Canteen.Api.Models;

namespace Canteen.Api.Endpoints;

public static class CanteenEndpoints
{
    public static void MapCanteenEndpoints(this WebApplication app)
    {
        app.MapGet("/api/menu", (InMemoryStore store) => store.GetMenuItems());

        app.MapGet("/api/orders", (InMemoryStore store) => store.GetOrders());

        app.MapPost("/api/orders", (PlaceOrderRequest request, InMemoryStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.EmployeeName))
            {
                return Results.BadRequest(new { message = "Employee name is required." });
            }

            try
            {
                var order = store.PlaceOrder(request.EmployeeName.Trim(), request.Items ?? new());
                return Results.Ok(order);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
    }
}
