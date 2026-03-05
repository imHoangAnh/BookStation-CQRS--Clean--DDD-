using BookStation.Application.Common;
using BookStation.Application.Queries.Orders;
using BookStation.Core.Entities.OrderAggregate;
using BookStation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Infrastructure.Queries;

public sealed class OrderQueryService : IOrderQueryService
{
    private readonly BookStationDbContext _dbContext;

    public OrderQueryService(BookStationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<OrderListDto>> GetAllDtoAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Payments)
            .ToListAsync(cancellationToken);

        return entities
            .OrderByDescending(o => o.CreatedAt)
            .Select(MapToOrderListDto)
            .ToList();
    }

    public async Task<OrderDetailDto?> GetByIdDtoAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return MapToOrderDetailDto(entity);
    }

    public async Task<PagedResult<OrderListDto>> GetPagedDtoAsync(
        long? userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Payments)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var entities = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = entities
            .Select(MapToOrderListDto)
            .ToList();

        return new PagedResult<OrderListDto>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<OrderListDto>> GetAllProjectionAsync(CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .AsNoTracking();

        return await query
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

    public async Task<OrderDetailDto?> GetByIdProjectionAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
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

    public async Task<PagedResult<OrderListDto>> GetPagedProjectionAsync(
        long? userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _dbContext.Orders
            .AsNoTracking()
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

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

    private static OrderListDto MapToOrderListDto(Order order)
    {
        return new OrderListDto(
            order.Id,
            order.UserId,
            order.Status,
            order.TotalAmount.Amount,
            order.DiscountAmount.Amount,
            order.FinalAmount.Amount,
            order.ItemCount);
    }

    private static OrderDetailDto MapToOrderDetailDto(Order order)
    {
        var items = order.Items
            .Select(i => new OrderItemDto(
                i.Id,
                i.BookVariantId,
                i.BookTitle,
                i.VariantName,
                i.Quantity,
                i.UnitPrice.Amount,
                i.Subtotal.Amount))
            .ToList();

        return new OrderDetailDto(
            order.Id,
            order.UserId,
            order.Status,
            order.TotalAmount.Amount,
            order.DiscountAmount.Amount,
            order.FinalAmount.Amount,
            order.ShippingAddress.Street,
            order.ShippingAddress.Ward,
            order.ShippingAddress.City,
            order.ShippingAddress.Country,
            order.ShippingAddress.PostalCode,
            order.Notes,
            order.ConfirmedAt,
            order.CompletedAt,
            order.CancelledAt,
            order.CancellationReason,
            items);
    }
}

