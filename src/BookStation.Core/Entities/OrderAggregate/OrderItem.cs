using BookStation.Core.SharedKernel;
using BookStation.Core.Entities.BookAggregate;
using BookStation.Core.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStation.Core.Entities.OrderAggregate;

public class OrderItem : Entity<long>
{
    public long OrderId { get; private set; }

    [ForeignKey(nameof(BookVariant))]
    public long BookVariantId { get; private set; } 
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public string BookTitle { get; private set; } = null!;
    public string VariantName { get; private set; } = null!;

    public Money Subtotal => UnitPrice * Quantity;
    public Order? Order { get; private set; }
    public BookVariant? BookVariant { get; private set; }
        
    private OrderItem() { }

    internal static OrderItem Create(long orderId, long variantId, int quantity, Money unitPrice, string bookTitle, string variantName)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        return new OrderItem { OrderId = orderId, BookVariantId = variantId, Quantity = quantity, UnitPrice = unitPrice, BookTitle = bookTitle, VariantName = variantName };
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(newQuantity));
        Quantity = newQuantity; UpdatedAt = DateTime.UtcNow;
    }
}


