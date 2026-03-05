using BookStation.Application.Common;

namespace BookStation.Application.Queries.Orders;

public interface IOrderQueryService
{
    // DTO style: load entities (optionally Include) then map in memory
    Task<IReadOnlyList<OrderListDto>> GetAllDtoAsync(CancellationToken cancellationToken = default);
    Task<OrderDetailDto?> GetByIdDtoAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedResult<OrderListDto>> GetPagedDtoAsync(
        long? userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    // Projection style: project directly in the database
    Task<IReadOnlyList<OrderListDto>> GetAllProjectionAsync(CancellationToken cancellationToken = default);
    Task<OrderDetailDto?> GetByIdProjectionAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedResult<OrderListDto>> GetPagedProjectionAsync(
        long? userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

