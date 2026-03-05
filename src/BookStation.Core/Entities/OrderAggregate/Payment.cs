using BookStation.Core.SharedKernel;
using BookStation.Core.Enums;
using BookStation.Core.ValueObjects;

namespace BookStation.Core.Entities.OrderAggregate;

/// <summary>
/// Payment entity representing a payment transaction for an order.
/// </summary>
public class Payment : Entity<long>
{
    public long OrderId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public Order? Order { get; private set; }

    private Payment() { }

    internal static Payment Create(long orderId, Money amount, PaymentMethod method)
    {
        return new Payment
        {
            OrderId = orderId,
            Amount = amount,
            Method = method,
            Status = PaymentStatus.Pending
        };
    }

    /// <summary>
    /// Marks the payment as completed.
    /// </summary>
    internal void MarkAsCompleted(string? transactionId = null)
    {
        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the payment as failed.
    /// </summary>
    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Refunds the payment. Only completed payments can be refunded.
    /// </summary>
    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Only completed payments can be refunded.");

        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }
}
