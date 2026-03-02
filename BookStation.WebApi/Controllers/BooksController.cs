using BookStation.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStation.Domain.Entities.BookAggregate;

namespace BookStation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookStationDbContext _dbContext;

    public BooksController(BookStationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookListDto>>> GetAllAsync(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
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
            .Select(b => new BookListDto
            {
                Id = b.Id,
                Title = b.Title,
                Language = b.Language,
                PublishYear = b.PublishYear
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<BookListDto>(items, totalCount, page, pageSize);

        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BookDetailDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var book = await _dbContext.Books
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookDetailDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Language = b.Language,
                PublishYear = b.PublishYear,
                PublisherId = b.PublisherId,
                CoverImageUrl = b.CoverImageUrl,
                PageCount = b.PageCount
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (book is null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [HttpGet("with-details")]
    public async Task<ActionResult<IEnumerable<BookWithDetailsDto>>> GetWithDetailsAsync(CancellationToken cancellationToken)
    {
        var books = await _dbContext.Books
            .AsNoTracking()
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Select(b => new BookWithDetailsDto
            {
                Id = b.Id,
                Title = b.Title,
                PublisherName = b.Publisher != null ? b.Publisher.Name : null,
                Authors = b.BookAuthors
                    .OrderBy(ba => ba.DisplayOrder)
                    .Select(ba => ba.Author != null ? ba.Author.FullName : string.Empty)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList(),
                Categories = b.BookCategories
                    .Select(bc => bc.Category != null ? bc.Category.Name : string.Empty)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return Ok(books);
    }

    [HttpGet("statistics/by-category")]
    public async Task<ActionResult<IEnumerable<BooksByCategoryDto>>> GetBooksByCategoryStatisticsAsync(CancellationToken cancellationToken)
    {
        var query =
            from category in _dbContext.Categories.AsNoTracking()
            join bookCategory in _dbContext.BookCategories.AsNoTracking()
                on category.Id equals bookCategory.CategoryId into categoryBooks
            from cb in categoryBooks.DefaultIfEmpty()
            join book in _dbContext.Books.AsNoTracking()
                on cb.BookId equals book.Id into booksGroup
            from b in booksGroup.DefaultIfEmpty()
            group b by new { category.Id, category.Name } into g
            select new BooksByCategoryDto
            {
                CategoryId = g.Key.Id,
                CategoryName = g.Key.Name,
                BookCount = g.Count(x => x != null)
            };

        var result = await query.ToListAsync(cancellationToken);

        return Ok(result);
    }
}

public sealed class BookListDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Language { get; init; }
    public int? PublishYear { get; init; }
}

public sealed class BookDetailDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public string? Language { get; init; }
    public int? PublishYear { get; init; }
    public Guid? PublisherId { get; init; }
    public string? CoverImageUrl { get; init; }
    public int? PageCount { get; init; }
}

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }

    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}

public sealed class BookWithDetailsDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string? PublisherName { get; init; }
    public List<string> Authors { get; init; } = [];
    public List<string> Categories { get; init; } = [];
}

public sealed class BooksByCategoryDto
{
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = null!;
    public int BookCount { get; init; }
}


