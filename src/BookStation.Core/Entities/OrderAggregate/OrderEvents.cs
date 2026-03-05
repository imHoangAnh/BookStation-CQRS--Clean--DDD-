using BookStation.Core.SharedKernel;
using BookStation.Core.Enums;

namespace BookStation.Core.Entities.OrderAggregate;

/// <summary>
/// Base event for order-related domain events.
/// </summary>
public abstract class OrderBaseEvent : DomainEvent
{
    public long OrderId { get; }

    protected OrderBaseEvent(long orderId)
    {
        OrderId = orderId;
    }
}

/// Event raised when a new order is created.
public sealed class OrderCreatedEvent : OrderBaseEvent
{
    public long UserId { get; }
    public string ShippingAddress { get; }

    public OrderCreatedEvent(Order order) : base(order.Id)
    {
        UserId = order.UserId;
        ShippingAddress = order.ShippingAddress.FullAddress;
    }
}

/// Event raised when a voucher is applied to an order.
public sealed class OrderVoucherAppliedEvent : OrderBaseEvent
{
    public long VoucherId { get; }
    public decimal DiscountAmount { get; }

    public OrderVoucherAppliedEvent(long orderId, long voucherId, decimal discountAmount) : base(orderId)
    {
        VoucherId = voucherId;
        DiscountAmount = discountAmount;
    }
}

/// Event raised when an order is confirmed.
public sealed class OrderConfirmedEvent : OrderBaseEvent
{
    public OrderConfirmedEvent(long orderId) : base(orderId) { }
}

/// Event raised when an order status changes.
public sealed class OrderStatusChangedEvent : OrderBaseEvent
{
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }

    public OrderStatusChangedEvent(long orderId, OrderStatus oldStatus, OrderStatus newStatus)
        : base(orderId)
    {
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

/// Event raised when an order is shipped.
public sealed class OrderShippedEvent : OrderBaseEvent
{
    public OrderShippedEvent(long orderId) : base(orderId) { }
}

/// Event raised when an order is delivered.
public sealed class OrderDeliveredEvent : OrderBaseEvent
{
    public OrderDeliveredEvent(long orderId) : base(orderId) { }
}

/// Event raised when an order is cancelled.
public sealed class OrderCancelledEvent : OrderBaseEvent
{
    public string Reason { get; }

    public OrderCancelledEvent(long orderId, string reason) : base(orderId)
    {
        Reason = reason;
    }
}

/// Event raised when payment is completed.
public sealed class OrderPaymentCompletedEvent : OrderBaseEvent
{
    public long PaymentId { get; }
    public decimal Amount { get; }

    public OrderPaymentCompletedEvent(long orderId, long paymentId, decimal amount) : base(orderId)
    {
        PaymentId = paymentId;
        Amount = amount;
    }
}

/// Event raised when an order is returned.
/// Rule: Return is independent of refund; refund is handled via Payment.Refund().
public sealed class OrderReturnedEvent : OrderBaseEvent
{
    public OrderReturnedEvent(long orderId) : base(orderId) { }
}

/// Event raised when an order is fully refunded.
public sealed class OrderRefundedEvent : OrderBaseEvent
{
    public OrderRefundedEvent(long orderId) : base(orderId) { }
}
