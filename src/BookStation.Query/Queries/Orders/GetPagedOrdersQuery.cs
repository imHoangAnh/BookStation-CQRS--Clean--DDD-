using BookStation.Query.Common;
using BookStation.Query.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Query.Queries.Orders;

public sealed record GetPagedOrdersQuery(
    long? UserId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<OrderListDto>>;

public sealed class GetPagedOrdersQueryHandler : IRequestHandler<GetPagedOrdersQuery, PagedResult<OrderListDto>>
{
    private readonly Abstractions.IReadDbContext _db;

    public GetPagedOrdersQueryHandler(Abstractions.IReadDbContext db) => _db = db;

    public async Task<PagedResult<OrderListDto>> Handle(
        GetPagedOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var query = _db.Orders.AsNoTracking().AsQueryable();

        if (request.UserId.HasValue)
            query = query.Where(o => o.UserId == request.UserId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderListDto(
                o.Id,
                o.UserId,
                o.Status,
                o.TotalAmount.Amount,
                o.DiscountAmount.Amount,
                o.FinalAmount.Amount,
                o.ItemCount))
            .ToListAsync(cancellationToken);

        return new PagedResult<OrderListDto>(items, totalCount, page, pageSize);
    }
}
