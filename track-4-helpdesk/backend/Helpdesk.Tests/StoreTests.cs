using Helpdesk.Api.Data;
using Helpdesk.Api.Models;
using Xunit;

namespace Helpdesk.Tests;

public class StoreTests
{
    [Fact]
    public void GetTickets_ReturnsSeedTickets()
    {
        var store = new InMemoryStore();

        Assert.NotEmpty(store.GetTickets());
    }

    [Fact]
    public void AddTicket_CreatesOpenTicket()
    {
        var store = new InMemoryStore();

        var ticket = store.AddTicket("Printer jammed", "Floor 1", "Asha", Priority.Medium);

        Assert.Equal("Open", ticket.Status);
        Assert.Contains(store.GetTickets(), t => t.Id == ticket.Id);
    }

    [Fact]
    public void AddTicket_EmptyTitle_IsRejected()
    {
        var store = new InMemoryStore();

        Assert.Throws<ArgumentException>(() => store.AddTicket("", "No title", "Asha", Priority.Low));
    }

    [Fact]
    public void GetTickets_MostUrgentComesFirst()
    {
        var store = new InMemoryStore();

        var tickets = store.GetTickets();

        Assert.Equal(Priority.Urgent, tickets[0].Priority);
        Assert.Equal(Priority.Low, tickets[^1].Priority);
    }

    [Fact]
    public void GetTickets_SamePriority_OlderTicketFirst()
    {
        var store = new InMemoryStore();
        var first = store.AddTicket("First high", "a", "Asha", Priority.High);
        var second = store.AddTicket("Second high", "b", "Ravi", Priority.High);

        var highs = store.GetTickets().Where(t => t.Priority == Priority.High && t.Id >= first.Id).ToList();

        Assert.Equal(first.Id, highs[0].Id);
        Assert.Equal(second.Id, highs[1].Id);
    }
}
