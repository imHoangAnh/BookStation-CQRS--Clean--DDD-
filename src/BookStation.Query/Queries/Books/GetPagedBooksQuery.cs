using BookStation.Query.Common;
using BookStation.Query.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Query.Queries.Books;

public sealed record GetPagedBooksQuery(
    string? Search,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<BookListDto>>;

public sealed class GetPagedBooksQueryHandler : IRequestHandler<GetPagedBooksQuery, PagedResult<BookListDto>>
{
    private readonly Abstractions.IReadDbContext _db;

    public GetPagedBooksQueryHandler(Abstractions.IReadDbContext db) => _db = db;

    public async Task<PagedResult<BookListDto>> Handle(
        GetPagedBooksQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var query = _db.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(b => b.Title.Contains(request.Search));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookListDto(b.Id, b.Title, b.Language, b.PublishYear))
            .ToListAsync(cancellationToken);

        return new PagedResult<BookListDto>(items, totalCount, page, pageSize);
    }
}
