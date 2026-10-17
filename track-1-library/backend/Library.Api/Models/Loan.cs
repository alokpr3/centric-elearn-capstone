namespace Library.Api.Models;

public record Loan(int Id, int BookId, string Borrower, DateTime BorrowDate, DateTime DueDate);

public record BorrowRequest(string Borrower);
