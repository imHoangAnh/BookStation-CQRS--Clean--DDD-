using BookStation.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Core.Entities.CartAggregate;
public class Cart : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public DateTime UpdateAt { get; private set; }

    // Navigation properties
    private readonly List<CartItem> _items = [];
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    /// Private constructor for EF Core.
    private Cart() { }
    /// <summary>
    /// Creates a new cart for a user.
    /// </summary>
    public static Cart Create(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        return new Cart
        {
            UserId = userId
        };
    }
    /// <summary>
    /// Adds an item to the cart.
    /// </summary>
    public void AddItem(long bookVariantId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        var existingItem = _items.Find(i => i.BookVariantId == bookVariantId);
        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            var newItem = CartItem.Create(bookVariantId, quantity);
            _items.Add(newItem);
        }
    }
    /// <summary>
    /// Removes an item from the cart.
    /// </summary>
    public void RemoveItem(long bookVariantId)
    {
        var item = _items.Find(i => i.BookVariantId == bookVariantId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }
}
