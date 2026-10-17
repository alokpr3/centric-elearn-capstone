namespace Helpdesk.Api.Models;

public enum Priority
{
    Low,
    Medium,
    High,
    Urgent
}

public record Ticket(
    int Id,
    string Title,
    string Description,
    string RaisedBy,
    Priority Priority,
    string Status,
    DateTime CreatedAt);

public record CreateTicketRequest(string Title, string Description, string RaisedBy, Priority Priority);
