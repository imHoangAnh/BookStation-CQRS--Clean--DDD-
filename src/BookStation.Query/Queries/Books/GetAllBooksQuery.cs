using BookStation.Query.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Query.Queries.Books;

public sealed record GetAllBooksQuery : IRequest<IReadOnlyList<BookListDto>>;

public sealed class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IReadOnlyList<BookListDto>>
{
    private readonly Abstractions.IReadDbContext _db;

    public GetAllBooksQueryHandler(Abstractions.IReadDbContext db) => _db = db;

    public async Task<IReadOnlyList<BookListDto>> Handle(
        GetAllBooksQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.Books
            .AsNoTracking()
            .OrderBy(b => b.Title)
            .Select(b => new BookListDto(b.Id, b.Title, b.Language, b.PublishYear))
            .ToListAsync(cancellationToken);
    }
}
