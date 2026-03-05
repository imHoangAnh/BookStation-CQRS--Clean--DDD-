using BookStation.Application.Queries.Books;
using BookStation.Application.Common;
using BookStation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Infrastructure.Queries;

public sealed class BookQueryService : IBookQueryService
{
    private readonly BookStationDbContext _dbContext;

    public BookQueryService(BookStationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<BookListDto>> GetAllDtoAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Books
            .AsNoTracking()
            .Include(b => b.Publisher)
            .ToListAsync(cancellationToken);

        return entities
            .OrderBy(b => b.Title)
            .Select(b => new BookListDto(
                b.Id,
                b.Title,
                b.Language,
                b.PublishYear))
            .ToList();
    }

    public async Task<BookDetailDto?> GetByIdDtoAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Books
            .AsNoTracking()
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return new BookDetailDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Language,
            entity.PublishYear,
            entity.PublisherId,
            entity.Publisher?.Name,
            entity.CoverImageUrl,
            entity.PageCount);
    }

    public async Task<PagedResult<BookListDto>> GetPagedDtoAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _dbContext.Books
            .AsNoTracking()
            .Include(b => b.Publisher)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Title.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var entities = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = entities
            .Select(b => new BookListDto(
                b.Id,
                b.Title,
                b.Language,
                b.PublishYear))
            .ToList();

        return new PagedResult<BookListDto>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<BookListDto>> GetAllProjectionAsync(CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Books
            .AsNoTracking();

        return await query
            .OrderBy(b => b.Title)
            .Select(b => new BookListDto(
                b.Id,
                b.Title,
                b.Language,
                b.PublishYear))
            .ToListAsync(cancellationToken);
    }

    public async Task<BookDetailDto?> GetByIdProjectionAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookDetailDto(
                b.Id,
                b.Title,
                b.Description,
                b.Language,
                b.PublishYear,
                b.PublisherId,
                b.Publisher != null ? b.Publisher.Name : null,
                b.CoverImageUrl,
                b.PageCount))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<BookListDto>> GetPagedProjectionAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _dbContext.Books
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Title.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookListDto(
                b.Id,
                b.Title,
                b.Language,
                b.PublishYear))
            .ToListAsync(cancellationToken);

        return new PagedResult<BookListDto>(items, totalCount, page, pageSize);
    }
}

