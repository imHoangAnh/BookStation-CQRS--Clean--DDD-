using BookStation.Core.SharedKernel;
using BookStation.Domain.Enums;
using BookStation.Domain.ValueObjects;

namespace BookStation.Domain.Entities.BookAggregate;

/// Book entity - Aggregate Root for book management.
public class Book : AggregateRoot<long>
{
    public string Title { get; private set; } = null!;
    public ISBN? ISBN { get; private set; }
    public string? Description { get; private set; }
    public string? Language { get; private set; }
    public int? PublishYear { get; private set; }
    public Guid? PublisherId { get; private set; }
    public Guid? SellerId { get; private set; }
    public BookStatus Status { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public int? PageCount { get; private set; }
    public string? CoAuthors { get; private set; }
    public string? Translators { get; private set; }

    // Navigation properties
    public Publisher? Publisher { get; private set; }

    private readonly List<BookVariant> _variants = [];
    public IReadOnlyList<BookVariant> Variants => _variants.AsReadOnly();

    private readonly List<BookAuthor> _bookAuthors = [];
    public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();

    private readonly List<BookCategory> _bookCategories = [];
    public IReadOnlyList<BookCategory> BookCategories => _bookCategories.AsReadOnly();

    private Book() { }

    public static Book Create(
        string title, ISBN? isbn = null, string? description = null,
        string? language = null, int? publishYear = null, Guid? publisherId = null,
        string? coverImageUrl = null, int? pageCount = null, Guid? sellerId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        var book = new Book
        {
            Title = title.Trim(), ISBN = isbn, Description = description?.Trim(),
            Language = language?.Trim(), PublishYear = publishYear, PublisherId = publisherId,
            CoverImageUrl = coverImageUrl, PageCount = pageCount, SellerId = sellerId,
            Status = BookStatus.Draft
        };
        book.AddDomainEvent(new BookCreatedEvent(book));
        return book;
    }

    public void Update(string title, ISBN? isbn, string? description, string? language,
        int? publishYear, Guid? publisherId, string? coverImageUrl, int? pageCount)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        Title = title.Trim(); ISBN = isbn; Description = description?.Trim();
        Language = language?.Trim(); PublishYear = publishYear; PublisherId = publisherId;
        CoverImageUrl = coverImageUrl; PageCount = pageCount; UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new BookUpdatedEvent(Id));
    }

    public BookVariant AddVariant(string variantName, Money price)
    {
        if (_variants.Any(v => v.VariantName.Equals(variantName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Variant '{variantName}' already exists.");
        var variant = BookVariant.Create(Id, variantName, price);
        _variants.Add(variant); UpdatedAt = DateTime.UtcNow;
        return variant;
    }

    public void RemoveVariant(long variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant != null) { _variants.Remove(variant); UpdatedAt = DateTime.UtcNow; }
    }

    public void AddAuthor(Guid authorId, int displayOrder = 1)
    {
        if (_bookAuthors.Any(ba => ba.AuthorId == authorId)) return;
        _bookAuthors.Add(BookAuthor.Create(Id, authorId, displayOrder));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAuthor(Guid authorId)
    {
        var bookAuthor = _bookAuthors.FirstOrDefault(ba => ba.AuthorId == authorId);
        if (bookAuthor != null) { _bookAuthors.Remove(bookAuthor); UpdatedAt = DateTime.UtcNow; }
    }

    public void AddCategory(int categoryId)
    {
        if (_bookCategories.Any(bc => bc.CategoryId == categoryId)) return;
        _bookCategories.Add(BookCategory.Create(Id, categoryId));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveCategory(int categoryId)
    {
        var bookCategory = _bookCategories.FirstOrDefault(bc => bc.CategoryId == categoryId);
        if (bookCategory != null) { _bookCategories.Remove(bookCategory); UpdatedAt = DateTime.UtcNow; }
    }

    public void Publish()
    {
        if (Status == BookStatus.Active) return;
        if (!_variants.Any()) throw new InvalidOperationException("Cannot publish a book without variants.");
        Status = BookStatus.Active; UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new BookPublishedEvent(Id));
    }

    public void Unpublish() { Status = BookStatus.Inactive; UpdatedAt = DateTime.UtcNow; }
    public void MarkOutOfStock() { Status = BookStatus.OutOfStock; UpdatedAt = DateTime.UtcNow; }
    public void Discontinue() { Status = BookStatus.Discontinued; UpdatedAt = DateTime.UtcNow; }

    public Money? GetMinPrice() => !_variants.Any() ? null : _variants.Min(v => v.Price);
    public Money? GetMaxPrice() => !_variants.Any() ? null : _variants.Max(v => v.Price);
}
