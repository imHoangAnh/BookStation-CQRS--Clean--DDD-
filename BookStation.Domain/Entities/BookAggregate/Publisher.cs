using BookStation.Core.SharedKernel;

namespace BookStation.Domain.Entities.CatalogAggregate;

/// Publisher entity.
public class Publisher : Entity<Guid>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public string? Website { get; private set; }
    public string? LogoUrl { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    private readonly List<Book> _books = [];
    public IReadOnlyList<Book> Books => _books.AsReadOnly();

    /// Private constructor for EF Core.
    private Publisher() { }
    /// Creates a new publisher.
    public static Publisher Create(string name, string? description = null, string? address = null, string? website = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        return new Publisher
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Address = address?.Trim(),
            Website = website?.Trim(),
            IsActive = true
        };
    }

    /// <summary>
    /// Updates the publisher.
    /// </summary>
    public void Update(string name, string? description, string? address, string? website, string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        Address = address?.Trim();
        Website = website?.Trim();
        LogoUrl = logoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the publisher.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the publisher.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the number of books by this publisher.
    /// </summary>
    public int BookCount => _books.Count;
}
