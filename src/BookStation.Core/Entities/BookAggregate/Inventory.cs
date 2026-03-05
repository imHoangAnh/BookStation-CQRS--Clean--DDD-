using BookStation.Core.SharedKernel;

namespace BookStation.Core.Entities.BookAggregate;

public class Inventory : Entity<long>
{
    public long BookVariantId { get; private set; }
    public int QuantityInStock { get; private set; }
    public int ReservedQuantity { get; private set; }

    public BookVariant? BookVariant { get; private set; }

    private Inventory() { }

    public static Inventory Create(long bookVariantId, int quantityInStock = 0)
    {
        return new Inventory { BookVariantId = bookVariantId, QuantityInStock = quantityInStock, ReservedQuantity = 0 };
    }

    public int AvailableQuantity => QuantityInStock - ReservedQuantity;

    public void AddStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        QuantityInStock += quantity; UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (quantity > AvailableQuantity) throw new InvalidOperationException("Not enough available stock.");
        QuantityInStock -= quantity; UpdatedAt = DateTime.UtcNow;
    }
}
