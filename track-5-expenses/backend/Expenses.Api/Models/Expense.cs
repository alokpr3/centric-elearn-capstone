namespace Expenses.Api.Models;

public record Expense(int Id, string Description, string PaidBy, decimal Amount, List<string> SharedWith);

public record PersonShare(string Person, decimal Share, decimal Owes);

public record CreateExpenseRequest(string Description, string PaidBy, decimal Amount, List<string> SharedWith);
