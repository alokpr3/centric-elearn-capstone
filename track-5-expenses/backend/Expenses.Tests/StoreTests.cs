using Expenses.Api.Data;
using Expenses.Api.Models;
using Xunit;

namespace Expenses.Tests;

public class StoreTests
{
    [Fact]
    public void AddExpense_StoresTheExpense()
    {
        var store = new InMemoryStore();

        var expense = store.AddExpense("Snacks", "Asha", 120m, new[] { "Asha", "Ravi" });

        Assert.Contains(store.GetExpenses(), e => e.Id == expense.Id);
    }

    [Fact]
    public void AddExpense_ZeroAmount_IsRejected()
    {
        var store = new InMemoryStore();

        Assert.Throws<ArgumentException>(() => store.AddExpense("Snacks", "Asha", 0m, new[] { "Asha" }));
    }

    [Fact]
    public void Split_EvenAmount_GivesEqualShares()
    {
        var expense = new Expense(1, "Lunch", "Asha", 90m, new List<string> { "Asha", "Ravi", "Meera" });

        var split = InMemoryStore.CalculateSplit(expense);

        Assert.All(split, s => Assert.Equal(30m, s.Share));
    }

    [Fact]
    public void Split_PayerOwesNothing()
    {
        var expense = new Expense(1, "Lunch", "Asha", 90m, new List<string> { "Asha", "Ravi", "Meera" });

        var split = InMemoryStore.CalculateSplit(expense);

        Assert.Equal(0m, split.First(s => s.Person == "Asha").Owes);
    }

    [Fact]
    public void Split_SharesAddUpToTheFullAmount()
    {
        var expense = new Expense(1, "Dinner", "Asha", 100m, new List<string> { "Asha", "Ravi", "Meera" });

        var split = InMemoryStore.CalculateSplit(expense);

        Assert.Equal(100m, split.Sum(s => s.Share));
    }
}
