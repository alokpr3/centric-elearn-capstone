using Helpdesk.Api.Data;
using Helpdesk.Api.Models;

namespace Helpdesk.Api.Endpoints;

public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this WebApplication app)
    {
        app.MapGet("/api/tickets", (InMemoryStore store) => store.GetTickets());

        app.MapPost("/api/tickets", (CreateTicketRequest request, InMemoryStore store) =>
        {
            try
            {
                var ticket = store.AddTicket(request.Summary, request.Description, request.RaisedBy, request.Priority);
                return Results.Created($"/api/tickets/{ticket.Id}", ticket);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
    }
}
