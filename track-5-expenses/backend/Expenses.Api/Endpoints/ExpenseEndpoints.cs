using Expenses.Api.Data;
using Expenses.Api.Models;

namespace Expenses.Api.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this WebApplication app)
    {
        app.MapGet("/api/expenses", (InMemoryStore store) => store.GetExpenses());

        app.MapPost("/api/expenses", (CreateExpenseRequest request, InMemoryStore store) =>
        {
            try
            {
                int id = store.AddExpense(request.Description, request.PaidBy, request.Amount, request.SharedWith ?? new());
                return Results.Created($"/api/expenses/{id}", store.FindExpense(id));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        app.MapGet("/api/expenses/{id:int}/split", (int id, InMemoryStore store) =>
        {
            var expense = store.FindExpense(id);
            return expense is null
                ? Results.NotFound(new { message = "Expense not found." })
                : Results.Ok(InMemoryStore.CalculateSplit(expense));
        });
    }
}
