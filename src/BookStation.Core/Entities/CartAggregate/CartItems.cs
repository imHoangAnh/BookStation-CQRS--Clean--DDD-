using BookStation.Core.SharedKernel;

namespace BookStation.Core.Entities.CartAggregate;

public class CartItem : Entity<long>
{
    public long BookVariantId { get; private set; }
    public int Quantity { get; private set; }

    private CartItem() { }

    internal static CartItem Create(long bookVariantId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        return new CartItem
        {
            BookVariantId = bookVariantId,
            Quantity = quantity
        };
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        Quantity += amount;
    }

    public void DecreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        if (amount > Quantity)
            throw new InvalidOperationException("Cannot decrease quantity below zero.");

        Quantity -= amount;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        Quantity = quantity;
    }
}
