using BookStation.Core.SharedKernel;

namespace BookStation.Core.Entities.BookAggregate;

public class Category : Entity<int>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public int? ParentCategoryId { get; private set; }

    public Category? ParentCategory { get; private set; }
    private readonly List<BookCategory> _bookCategories = [];
    public IReadOnlyList<BookCategory> BookCategories => _bookCategories.AsReadOnly();

    private Category() { }

    public static Category Create(string name, string? description = null, int? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        return new Category { Name = name.Trim(), Description = description?.Trim(), ParentCategoryId = parentCategoryId };
    }
}
