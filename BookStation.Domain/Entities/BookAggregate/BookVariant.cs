using BookStation.Core.SharedKernel;
using BookStation.Domain.ValueObjects;

namespace BookStation.Domain.Entities.BookAggregate;

public class BookVariant : Entity<long>
{
    public long BookId { get; private set; }
    public string VariantName { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public Money? OriginalPrice { get; private set; }
    public int? WeightGrams { get; private set; }
    public string? SKU { get; private set; } // Stock Keeping Unit
    public bool IsAvailable { get; private set; }

    // Navigation properties
    public Book? Book { get; private set; }
    public Inventory? Inventory { get; private set; }

    private BookVariant() { }

    internal static BookVariant Create(long bookId, string variantName, Money price)
    {
        if (string.IsNullOrWhiteSpace(variantName))
            throw new ArgumentException("Variant name cannot be empty.", nameof(variantName));
        return new BookVariant { BookId = bookId, VariantName = variantName.Trim(), Price = price, IsAvailable = true };
    }

    public void Update(string variantName, Money price, Money? originalPrice = null, int? weightGrams = null, string? sku = null)
    {
        if (string.IsNullOrWhiteSpace(variantName))
            throw new ArgumentException("Variant name cannot be empty.", nameof(variantName));
        VariantName = variantName.Trim(); Price = price; OriginalPrice = originalPrice;
        WeightGrams = weightGrams; SKU = sku?.Trim(); UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(Money newPrice, Money? originalPrice = null)
    {
        OriginalPrice = originalPrice ?? Price; Price = newPrice; UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvailability(bool isAvailable) { IsAvailable = isAvailable; UpdatedAt = DateTime.UtcNow; }
    public bool HasDiscount => OriginalPrice != null && OriginalPrice > Price;

    public decimal GetDiscountPercentage()
    {
        if (!HasDiscount || OriginalPrice == null) return 0;
        return Math.Round((1 - Price.Amount / OriginalPrice.Amount) * 100, 2);
    }
}
