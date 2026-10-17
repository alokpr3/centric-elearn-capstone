using Expenses.Api.Models;

namespace Expenses.Api.Data;

public class InMemoryStore
{
    private readonly List<Expense> _expenses = new()
    {
        new Expense(1, "Team lunch", "Asha", 900m, new List<string> { "Asha", "Ravi", "Meera" }),
        new Expense(2, "Cab to client office", "Ravi", 450m, new List<string> { "Ravi", "Meera" }),
    };

    public IReadOnlyList<Expense> GetExpenses() => _expenses;

    public Expense? FindExpense(int id) => _expenses.FirstOrDefault(e => e.Id == id);

    public Expense AddExpense(string description, string paidBy, decimal amount, IEnumerable<string> sharedWith)
    {
        var people = sharedWith
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .ToList();

        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be more than zero.");
        }

        if (people.Count == 0)
        {
            throw new ArgumentException("Choose at least one person to share with.");
        }

        var expense = new Expense(_expenses.Count + 1, description, paidBy, amount, people);
        _expenses.Add(expense);
        return expense;
    }

    public static IReadOnlyList<PersonShare> CalculateSplit(Expense expense)
    {
        var each = Math.Round(expense.Amount / expense.SharedWith.Count, 2);

        return expense.SharedWith
            .Select(person => new PersonShare(
                person,
                each,
                person == expense.PaidBy ? 0m : each))
            .ToList();
    }
}
