using BookStation.Core.SharedKernel;
using BookStation.Domain.Enums;
using BookStation.Domain.ValueObjects;

namespace BookStation.Domain.Entities.OrderAggregate;

/// <summary>
/// Order entity - Aggregate Root for order management.
/// </summary>
public class Order : AggregateRoot<long>
{
    /// <summary>
    /// Gets the user ID.
    /// </summary>
    public long UserId { get; private set; }

    /// <summary>
    /// Gets the total amount before discount.
    /// </summary>
    public Money TotalAmount { get; private set; } = Money.Zero();

    /// <summary>
    /// Gets the discount amount.
    /// </summary>
    public Money DiscountAmount { get; private set; } = Money.Zero();

    /// <summary>
    /// Gets the final amount after discount.
    /// </summary>
    public Money FinalAmount { get; private set; } = Money.Zero();

    /// <summary>
    /// Gets the order status.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Gets the voucher ID if applied.
    /// </summary>
    public long? VoucherId { get; private set; }

    /// <summary>
    /// Gets the shipping address.
    /// </summary>
    public Address ShippingAddress { get; private set; } = null!;

    /// <summary>
    /// Gets any notes for the order.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Gets the order confirmation date.
    /// </summary>
    public DateTime? ConfirmedAt { get; private set; }

    /// <summary>
    /// Gets the order completion date.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Gets the cancellation date.
    /// </summary>
    public DateTime? CancelledAt { get; private set; }

    /// <summary>
    /// Gets the cancellation reason.
    /// </summary>
    public string? CancellationReason { get; private set; }

    // Navigation properties
    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private readonly List<Payment> _payments = [];
    public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private Order() { }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    public static Order Create(long userId, Address shippingAddress, string? notes = null)
    {
        var order = new Order
        {
            UserId = userId,
            ShippingAddress = shippingAddress,
            Notes = notes,
            Status = OrderStatus.Pending,
            TotalAmount = Money.Zero(),
            DiscountAmount = Money.Zero(),
            FinalAmount = Money.Zero()
        };

        order.AddDomainEvent(new OrderCreatedEvent(order));

        return order;
    }

    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    public OrderItem AddItem(long variantId, int quantity, Money unitPrice, string bookTitle, string variantName)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot add items to a non-pending order.");

        var existingItem = _items.FirstOrDefault(i => i.BookVariantId == variantId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var item = OrderItem.Create(Id, variantId, quantity, unitPrice, bookTitle, variantName);
            _items.Add(item);
        }

        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;

        return _items.First(i => i.BookVariantId == variantId);
    }

    /// <summary>
    /// Removes an item from the order.
    /// </summary>
    public void RemoveItem(long variantId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot remove items from a non-pending order.");

        var item = _items.FirstOrDefault(i => i.BookVariantId == variantId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Applies a voucher to the order.
    /// </summary>
    public void ApplyVoucher(long voucherId, Money discountAmount)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot apply voucher to a non-pending order.");

        VoucherId = voucherId;
        DiscountAmount = discountAmount;
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderVoucherAppliedEvent(Id, voucherId, discountAmount.Amount));
    }

    /// <summary>
    /// Removes the applied voucher.
    /// </summary>
    public void RemoveVoucher()
    {
        if (VoucherId == null)
            return;

        VoucherId = null;
        DiscountAmount = Money.Zero();
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Confirms the order.
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot confirm an order without items.");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderConfirmedEvent(Id));
    }

    /// <summary>
    /// Starts processing the order.
    /// </summary>
    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can start processing.");

        Status = OrderStatus.Processing;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderStatusChangedEvent(Id, OrderStatus.Confirmed, OrderStatus.Processing));
    }

    /// <summary>
    /// Marks the order as shipped.
    /// </summary>
    public void Ship()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Only processing orders can be shipped.");

        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderShippedEvent(Id));
    }

    /// <summary>
    /// Marks the order as delivered.
    /// </summary>
    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderDeliveredEvent(Id));
    }

    /// <summary>
    /// Cancels the order.
    /// </summary>
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel delivered or already cancelled orders.");

        var oldStatus = Status;
        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderCancelledEvent(Id, reason));
    }

    /// <summary>
    /// Adds a payment to the order.
    /// </summary>
    public Payment AddPayment(Money amount, PaymentMethod method)
    {
        var payment = Payment.Create(Id, amount, method);
        _payments.Add(payment);
        UpdatedAt = DateTime.UtcNow;

        return payment;
    }

    /// <summary>
    /// Completes a payment on this order.
    /// </summary>
    public void CompletePayment(long paymentId, string? transactionId = null)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId)
            ?? throw new InvalidOperationException($"Payment {paymentId} not found on this order.");

        payment.MarkAsCompleted(transactionId);

        AddDomainEvent(new OrderPaymentCompletedEvent(Id, paymentId, payment.Amount.Amount));
    }

    /// <summary>
    /// Marks the order as returned.
    /// Business rule: only delivered orders can be returned.
    /// </summary>
    public void Return()
    {
        if (Status != OrderStatus.Delivered)
            throw new InvalidOperationException("Only delivered orders can be returned.");

        Status = OrderStatus.Returned;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderReturnedEvent(Id));
    }

    /// <summary>
    /// Marks the order as refunded after all payments have been refunded.
    /// </summary>
    public void MarkAsRefunded()
    {
        if (Status != OrderStatus.Returned && Status != OrderStatus.Cancelled)
            throw new InvalidOperationException("Only returned or cancelled orders can be refunded.");

        Status = OrderStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderRefundedEvent(Id));
    }

    /// <summary>
    /// Gets the total paid amount.
    /// </summary>
    public Money TotalPaid => _payments
        .Where(p => p.Status == PaymentStatus.Completed)
        .Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);

    /// <summary>
    /// Gets a value indicating whether the order is fully paid.
    /// </summary>
    public bool IsFullyPaid => TotalPaid >= FinalAmount;

    /// <summary>
    /// Gets a value indicating whether the order can be cancelled.
    /// </summary>
    public bool CanBeCancelled => Status is OrderStatus.Pending or OrderStatus.Confirmed or OrderStatus.Processing;

    /// <summary>
    /// Gets the item count.
    /// </summary>
    public int ItemCount => _items.Sum(i => i.Quantity);

    private void RecalculateTotal()
    {
        TotalAmount = _items.Aggregate(Money.Zero(), (sum, item) => sum + item.Subtotal);
        FinalAmount = TotalAmount - DiscountAmount;
        if (FinalAmount.Amount < 0)
        {
            FinalAmount = Money.Zero();
        }
    }
}
