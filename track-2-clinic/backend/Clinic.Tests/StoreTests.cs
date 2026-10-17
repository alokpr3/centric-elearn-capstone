using Clinic.Api.Data;
using Xunit;

namespace Clinic.Tests;

public class StoreTests
{
    private static readonly DateTime NineAm = new(2030, 1, 7, 9, 0, 0);

    [Fact]
    public void GetDoctors_ReturnsSeedDoctors()
    {
        var store = new InMemoryStore();

        Assert.NotEmpty(store.GetDoctors());
    }

    [Fact]
    public void Book_FreeSlot_CreatesAppointment()
    {
        var store = new InMemoryStore();

        var appointment = store.Book(1, "Asha", NineAm, 30);

        Assert.NotNull(appointment);
        Assert.Equal(NineAm.AddMinutes(30), appointment!.End);
    }

    [Fact]
    public void Book_SameSlotTwice_IsRejected()
    {
        var store = new InMemoryStore();
        store.Book(1, "Asha", NineAm, 30);

        Assert.Throws<InvalidOperationException>(() => store.Book(1, "Ravi", NineAm, 30));
    }

    [Fact]
    public void Book_StartsInsideAnotherVisit_IsRejected()
    {
        var store = new InMemoryStore();
        store.Book(1, "Asha", NineAm, 30);

        Assert.Throws<InvalidOperationException>(() => store.Book(1, "Ravi", NineAm.AddMinutes(15), 30));
    }

    [Fact]
    public void Book_SameTimeWithDifferentDoctor_IsAllowed()
    {
        var store = new InMemoryStore();
        store.Book(1, "Asha", NineAm, 30);

        var second = store.Book(2, "Ravi", NineAm, 30);

        Assert.NotNull(second);
    }
}
