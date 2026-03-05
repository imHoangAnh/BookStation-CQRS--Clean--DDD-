using BookStation.Application.Common;

namespace BookStation.Application.Queries.Books;

public interface IBookQueryService
{
    // DTO style: load entities (optionally Include) then map in memory
    Task<IReadOnlyList<BookListDto>> GetAllDtoAsync(CancellationToken cancellationToken = default);
    Task<BookDetailDto?> GetByIdDtoAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedResult<BookListDto>> GetPagedDtoAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    // Projection style: project directly in the database
    Task<IReadOnlyList<BookListDto>> GetAllProjectionAsync(CancellationToken cancellationToken = default);
    Task<BookDetailDto?> GetByIdProjectionAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedResult<BookListDto>> GetPagedProjectionAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

