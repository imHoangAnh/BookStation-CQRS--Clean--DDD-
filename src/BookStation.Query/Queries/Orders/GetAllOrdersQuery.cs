using BookStation.Query.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Query.Queries.Orders;

public sealed record GetAllOrdersQuery : IRequest<IReadOnlyList<OrderListDto>>;

public sealed class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IReadOnlyList<OrderListDto>>
{
    private readonly Abstractions.IReadDbContext _db;

    public GetAllOrdersQueryHandler(Abstractions.IReadDbContext db) => _db = db;

    public async Task<IReadOnlyList<OrderListDto>> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderListDto(
                o.Id,
                o.UserId,
                o.Status,
                o.TotalAmount.Amount,
                o.DiscountAmount.Amount,
                o.FinalAmount.Amount,
                o.ItemCount))
            .ToListAsync(cancellationToken);
    }
}
