namespace BookStation.Domain.Entities.BookAggregate;

public class BookAuthor
{
    public long BookId { get; private set; }
    public Guid AuthorId { get; private set; }
    public int DisplayOrder { get; private set; }

    // Navigation properties
    public Book? Book { get; private set; }
    public Author? Author { get; private set; }

    private BookAuthor() { }

    internal static BookAuthor Create(long bookId, Guid authorId, int displayOrder = 1)
    {
        return new BookAuthor { BookId = bookId, AuthorId = authorId, DisplayOrder = displayOrder };
    }

    public void Update(int displayOrder) { DisplayOrder = displayOrder; }
}
