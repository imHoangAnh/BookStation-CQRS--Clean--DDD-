using BookStation.Core.Enums;

namespace BookStation.Application.Queries.Orders;

public sealed record OrderListDto(
    long Id,
    long UserId,
    OrderStatus Status,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    int ItemCount);

public sealed record OrderDetailDto(
    long Id,
    long UserId,
    OrderStatus Status,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    string ShippingStreet,
    string ShippingWard,
    string ShippingCity,
    string ShippingCountry,
    string? ShippingPostalCode,
    string? Notes,
    DateTime? ConfirmedAt,
    DateTime? CompletedAt,
    DateTime? CancelledAt,
    string? CancellationReason,
    IReadOnlyList<OrderItemDto> Items);

public sealed record OrderItemDto(
    long Id,
    long BookVariantId,
    string BookTitle,
    string VariantName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);

