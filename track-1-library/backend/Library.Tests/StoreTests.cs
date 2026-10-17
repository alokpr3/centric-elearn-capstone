using Library.Api.Data;
using Xunit;

namespace Library.Tests;

public class StoreTests
{
    [Fact]
    public void GetBooks_ReturnsSeedBooks()
    {
        var store = new InMemoryStore();

        Assert.NotEmpty(store.GetBooks());
    }

    [Fact]
    public void Borrow_ReducesCopiesByOne()
    {
        var store = new InMemoryStore();
        var before = store.FindBook(1)!.CopiesAvailable;

        store.Borrow(1, "Asha", new DateTime(2025, 3, 1));

        Assert.Equal(before - 1, store.FindBook(1)!.CopiesAvailable);
    }

    [Fact]
    public void Borrow_UnknownBook_ReturnsNull()
    {
        var store = new InMemoryStore();

        Assert.Null(store.Borrow(999, "Asha", new DateTime(2025, 3, 1)));
    }

    [Fact]
    public void DueDate_Is14DaysAfterBorrowDate()
    {
        var due = InMemoryStore.CalculateDueDate(new DateTime(2025, 3, 1));

        Assert.Equal(new DateTime(2025, 3, 15), due);
    }

    [Fact]
    public void Fine_IsChargedPerLateDay()
    {
        var fine = InMemoryStore.CalculateFine(new DateTime(2025, 3, 15), new DateTime(2025, 3, 18));

        Assert.Equal(1.50m, fine);
    }
}
