using Library.Api.Data;
using Library.Api.Models;

namespace Library.Api.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapGet("/api/books", (InMemoryStore store) => store.GetBooks());

        app.MapPost("/api/books/{id}/borrow", (string id, BorrowRequest request, InMemoryStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.Borrower))
            {
                return Results.BadRequest(new { message = "Borrower name is required." });
            }

            try
            {
                var loan = store.Borrow(id, request.Borrower.Trim(), DateTime.Today);
                return loan is null
                    ? Results.NotFound(new { message = "Book not found." })
                    : Results.Ok(loan);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
        });
    }
}
