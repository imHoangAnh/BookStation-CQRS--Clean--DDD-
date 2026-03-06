using BookStation.Query.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Query.Queries.Books;

public sealed record GetBookByIdQuery(long Id) : IRequest<BookDetailDto?>;

public sealed class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDetailDto?>
{
    private readonly Abstractions.IReadDbContext _db;

    public GetBookByIdQueryHandler(Abstractions.IReadDbContext db) => _db = db;

    public async Task<BookDetailDto?> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.Books
            .AsNoTracking()
            .Where(b => b.Id == request.Id)
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
}
