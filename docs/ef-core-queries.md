## EF Core Query Examples in BookStation

### AsNoTracking with simple projection

```csharp
var items = await _dbContext.Books
    .AsNoTracking()
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
```

### Include / ThenInclude for related data

```csharp
var books = await _dbContext.Books
    .AsNoTracking()
    .Include(b => b.Publisher)
    .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
    .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
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
```

### GroupBy with left join for category statistics

```csharp
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
```

### Pagination with PagedResult wrapper

```csharp
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
```

