using BookStation.Core.SharedKernel;

namespace BookStation.Domain.Entities.CatalogAggregate;

/// <summary>
/// Author entity.
/// </summary>
public class Author : Entity<Guid>
{
    public string FullName { get; private set; } = null!;
    public string? Bio { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public DateTime? DiedDate { get; private set; }
    public string? Address { get; private set; }
    public string? Country { get; private set; }
    // Gets the author's photo URL.
    public string? PhotoUrl { get; private set; }

    // Navigation properties - using backing fields + IReadOnlyList to prevent noone can modify the collection outside the aggregate. 
    private readonly List<BookAuthor> _bookAuthors = [];
    public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();

    // Private constructor for EF Core.
    private Author() { }

    // Creates a new author.
    public static Author Create(
        string fullName,
        string? bio = null,
        DateTime? dateOfBirth = null,
        string? country = null,
        Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));

        return new Author
        {
            FullName = fullName.Trim(), // Trim() help you remove leading and trailing whitespace to get db consistency
            Bio = bio?.Trim(),
            DateOfBirth = dateOfBirth,
            Country = country?.Trim(),
            UserId = userId
        };
    }

    /// Updates the author information.
    public void Update(
        string fullName,
        string? bio,
        DateTime? dateOfBirth,
        DateTime? diedDate,
        string? address,
        string? country,
        string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));

        FullName = fullName.Trim();
        Bio = bio?.Trim();
        DateOfBirth = dateOfBirth;
        DiedDate = diedDate;
        Address = address?.Trim();
        Country = country?.Trim();
        PhotoUrl = photoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

}
