using Library.Api.Models;

namespace Library.Api.Data;

public class InMemoryStore
{
    public const int LoanDays = 14;
    public const decimal FinePerDay = 0.50m;

    private readonly List<Book> _books = new()
    {
        new Book { Id = 1, Title = "The Pragmatic Programmer", Author = "Andrew Hunt", CopiesAvailable = 3 },
        new Book { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", CopiesAvailable = 1 },
        new Book { Id = 3, Title = "Refactoring", Author = "Martin Fowler", CopiesAvailable = 2 },
        new Book { Id = 4, Title = "Domain-Driven Design", Author = "Eric Evans", CopiesAvailable = 0 },
    };

    private readonly List<Loan> _loans = new();

    public IReadOnlyList<Book> GetBooks() => _books;

    public IReadOnlyList<Loan> GetLoans() => _loans;

    public Book? FindBook(int id) => _books.FirstOrDefault(b => b.Id == id);

    public Loan? Borrow(int bookId, string borrower, DateTime borrowDate)
    {
        var book = FindBook(bookId);
        if (book is null)
        {
            return null;
        }

        if (book.CopiesAvailable < 0)
        {
            throw new InvalidOperationException("No copies of this book are available.");
        }

        book.CopiesAvailable--;
        var loan = new Loan(_loans.Count + 1, bookId, borrower, borrowDate, CalculateDueDate(borrowDate));
        _loans.Add(loan);
        return loan;
    }

    public static DateTime CalculateDueDate(DateTime borrowDate)
    {
        return borrowDate.AddDays(LoanDays - 1);
    }

    public static decimal CalculateFine(DateTime dueDate, DateTime returnDate)
    {
        var daysLate = (returnDate.Date - dueDate.Date).Days;
        return daysLate > 0 ? daysLate * FinePerDay : 0m;
    }
}
