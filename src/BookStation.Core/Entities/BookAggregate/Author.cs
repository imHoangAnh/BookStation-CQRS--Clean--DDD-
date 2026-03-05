using BookStation.Core.SharedKernel;

namespace BookStation.Core.Entities.BookAggregate;

public class Author : Entity<Guid>
{
    public string FullName { get; private set; } = null!;
    public string? Bio { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public DateTime? DiedDate { get; private set; }
    public string? Address { get; private set; }
    public string? Country { get; private set; }
    public string? PhotoUrl { get; private set; }
    public Guid? UserId { get; private set; }

    private readonly List<BookAuthor> _bookAuthors = [];
    public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();

    private Author() { }

    public static Author Create(string fullName, string? bio = null, DateTime? dateOfBirth = null, string? country = null, Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));
        return new Author { FullName = fullName.Trim(), Bio = bio?.Trim(), DateOfBirth = dateOfBirth, Country = country?.Trim(), UserId = userId };
    }

    public void Update(string fullName, string? bio, DateTime? dateOfBirth, DateTime? diedDate, string? address, string? country, string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));
        FullName = fullName.Trim(); Bio = bio?.Trim(); DateOfBirth = dateOfBirth;
        DiedDate = diedDate; Address = address?.Trim(); Country = country?.Trim();
        PhotoUrl = photoUrl; UpdatedAt = DateTime.UtcNow;
    }
}
