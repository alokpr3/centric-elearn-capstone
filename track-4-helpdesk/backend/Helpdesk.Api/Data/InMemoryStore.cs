using Helpdesk.Api.Models;

namespace Helpdesk.Api.Data;

public class InMemoryStore
{
    private readonly List<Ticket> _tickets = new()
    {
        new Ticket(1, "Cannot connect to VPN", "Error 809 since this morning.", "Meera", Priority.High, "Open", new DateTime(2025, 3, 3, 9, 15, 0)),
        new Ticket(2, "Need a second monitor", "For the design team desk.", "Rohan", Priority.Low, "Open", new DateTime(2025, 3, 3, 10, 0, 0)),
        new Ticket(3, "Email server down", "Nobody on floor 2 can send mail.", "Lena", Priority.Urgent, "Open", new DateTime(2025, 3, 3, 11, 30, 0)),
        new Ticket(4, "Laptop battery drains fast", "Lasts only one hour.", "Ishaan", Priority.Medium, "Open", new DateTime(2025, 3, 4, 8, 45, 0)),
    };

    public IReadOnlyList<Ticket> GetTickets()
    {
        return _tickets
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToList();
    }

    public Ticket AddTicket(string title, string description, string raisedBy, Priority priority)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentException("A ticket needs a title.");
        }

        var ticket = new Ticket(
            _tickets.Count + 1,
            title,
            description,
            raisedBy,
            priority,
            "Open",
            DateTime.Now);
        _tickets.Add(ticket);
        return ticket;
    }
}
