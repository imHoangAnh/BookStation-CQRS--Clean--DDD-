namespace BookStation.Domain.Entities.BookAggregate;

public class BookCategory
{
    public long BookId { get; private set; }
    public int CategoryId { get; private set; }

    // Navigation properties
    public Book? Book { get; private set; }
    public Category? Category { get; private set; }

    private BookCategory() { }

    internal static BookCategory Create(long bookId, int categoryId)
    {
        return new BookCategory { BookId = bookId, CategoryId = categoryId };
    }
}
