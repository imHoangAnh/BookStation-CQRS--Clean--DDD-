using BookStation.Application.Queries.Books;
using BookStation.Application.Queries.Common;
using Microsoft.AspNetCore.Mvc;

namespace BookStation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookQueryService _bookQueryService;

    public BooksController(IBookQueryService bookQueryService)
    {
        _bookQueryService = bookQueryService;
    }

    // DTO-style: load entities + map in memory
    [HttpGet("dto")]
    public async Task<ActionResult<IReadOnlyList<BookListDto>>> GetAllDtoAsync(CancellationToken cancellationToken)
    {
        var result = await _bookQueryService.GetAllDtoAsync(cancellationToken);
        return Ok(result);
    }

    // Projection-style: project directly to DTO in database
    [HttpGet("projection")]
    public async Task<ActionResult<IReadOnlyList<BookListDto>>> GetAllProjectionAsync(CancellationToken cancellationToken)
    {
        var result = await _bookQueryService.GetAllProjectionAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("paged/dto")]
    public async Task<ActionResult<PagedResult<BookListDto>>> GetPagedDtoAsync(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _bookQueryService.GetPagedDtoAsync(search, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("paged/projection")]
    public async Task<ActionResult<PagedResult<BookListDto>>> GetPagedProjectionAsync(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _bookQueryService.GetPagedProjectionAsync(search, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}/dto")]
    public async Task<ActionResult<BookDetailDto>> GetByIdDtoAsync(long id, CancellationToken cancellationToken)
    {
        var result = await _bookQueryService.GetByIdDtoAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("{id:long}/projection")]
    public async Task<ActionResult<BookDetailDto>> GetByIdProjectionAsync(long id, CancellationToken cancellationToken)
    {
        var result = await _bookQueryService.GetByIdProjectionAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}

