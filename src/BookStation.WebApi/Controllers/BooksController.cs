using BookStation.Application.Books.Commands;
using BookStation.Query.Common;
using BookStation.Query.Queries.Books;
using BookStation.WebApi.Contracts.Books;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all books with pagination.
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(Pagination<BookListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1)
    {
        var query = new GetAllBooksQuery(pageNumber);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Search books with filters and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Pagination<BookListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] SearchBooksQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get book details by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var query = new GetBookByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Create a new book.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CreateBookResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateBookCommand command)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userId, out var sellerId))
        {
            command = command with { SellerId = sellerId };
        }

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.BookId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Add a variant to a book.
    /// </summary>
    [HttpPost("{id}/variants")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddVariant(long id, [FromBody] AddBookVariantCommand command)
    {
        if (id != command.BookId)
        {
            return BadRequest(new { error = "Book ID in URL mismatch with body." });
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userId, out var sellerId))
        {
            command = command with { SellerId = sellerId };
        }

        try
        {
            var variantId = await _mediator.Send(command);
            return Ok(new { variantId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Publish a book (make it active).
    /// </summary>
    [HttpPost("{id}/publish")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(long id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? sellerId = null;
        if (Guid.TryParse(userId, out var sId))
        {
            sellerId = sId;
        }

        var command = new PublishBookCommand(id, sellerId);

        try
        {
            await _mediator.Send(command);
            return Ok(new { message = "Book published successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Uploads a book cover image.
    /// </summary>
    [HttpPost("cover-image")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadCoverImage([FromForm] UploadCoverImageRequest request, [FromQuery] long? bookId = null)
    {
        var file = request.File;
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file uploaded." });

        var command = new UploadBookCoverCommand(file, bookId);

        try
        {
            var url = await _mediator.Send(command);
            return Ok(new { url });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Book with ID {bookId} not found." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
