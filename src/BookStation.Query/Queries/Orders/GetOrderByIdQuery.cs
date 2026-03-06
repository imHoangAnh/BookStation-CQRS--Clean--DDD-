using BookStation.Query.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Query.Queries.Orders;

public sealed record GetOrderByIdQuery(long Id) : IRequest<OrderDetailDto?>;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto?>
{
    private readonly Abstractions.IReadDbContext _db;

    public GetOrderByIdQueryHandler(Abstractions.IReadDbContext db) => _db = db;

    public async Task<OrderDetailDto?> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.Orders
            .AsNoTracking()
            .Where(o => o.Id == request.Id)
            .Select(o => new OrderDetailDto(
                o.Id,
                o.UserId,
                o.Status,
                o.TotalAmount.Amount,
                o.DiscountAmount.Amount,
                o.FinalAmount.Amount,
                o.ShippingAddress.Street,
                o.ShippingAddress.Ward,
                o.ShippingAddress.City,
                o.ShippingAddress.Country,
                o.ShippingAddress.PostalCode,
                o.Notes,
                o.ConfirmedAt,
                o.CompletedAt,
                o.CancelledAt,
                o.CancellationReason,
                o.Items
                    .Select(i => new OrderItemDto(
                        i.Id,
                        i.BookVariantId,
                        i.BookTitle,
                        i.VariantName,
                        i.Quantity,
                        i.UnitPrice.Amount,
                        i.Subtotal.Amount))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
