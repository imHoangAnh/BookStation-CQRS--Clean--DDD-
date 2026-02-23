using BookStation.Core.SharedKernel;

namespace BookStation.Domain.Entities.BookAggregate;

public class Publisher : Entity<Guid>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public string? Website { get; private set; }
    public string? LogoUrl { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Book> _books = [];
    public IReadOnlyList<Book> Books => _books.AsReadOnly();

    private Publisher() { }

    public static Publisher Create(string name, string? description = null, string? address = null, string? website = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        return new Publisher { Name = name.Trim(), Description = description?.Trim(), Address = address?.Trim(), Website = website?.Trim(), IsActive = true };
    }

    public void Update(string name, string? description, string? address, string? website, string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name.Trim(); Description = description?.Trim(); Address = address?.Trim();
        Website = website?.Trim(); LogoUrl = logoUrl; UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
    public int BookCount => _books.Count;
}
