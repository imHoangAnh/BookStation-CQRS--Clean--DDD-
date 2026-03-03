using BookStation.Application.Queries.Common;
using BookStation.Application.Queries.Orders;
using Microsoft.AspNetCore.Mvc;

namespace BookStation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderQueryService _orderQueryService;

    public OrdersController(IOrderQueryService orderQueryService)
    {
        _orderQueryService = orderQueryService;
    }

    // DTO-style: load entities + map in memory
    [HttpGet("dto")]
    public async Task<ActionResult<IReadOnlyList<OrderListDto>>> GetAllDtoAsync(CancellationToken cancellationToken)
    {
        var result = await _orderQueryService.GetAllDtoAsync(cancellationToken);
        return Ok(result);
    }

    // Projection-style: project directly to DTO in database
    [HttpGet("projection")]
    public async Task<ActionResult<IReadOnlyList<OrderListDto>>> GetAllProjectionAsync(CancellationToken cancellationToken)
    {
        var result = await _orderQueryService.GetAllProjectionAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("paged/dto")]
    public async Task<ActionResult<PagedResult<OrderListDto>>> GetPagedDtoAsync(
        [FromQuery] long? userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _orderQueryService.GetPagedDtoAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("paged/projection")]
    public async Task<ActionResult<PagedResult<OrderListDto>>> GetPagedProjectionAsync(
        [FromQuery] long? userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _orderQueryService.GetPagedProjectionAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}/dto")]
    public async Task<ActionResult<OrderDetailDto>> GetByIdDtoAsync(long id, CancellationToken cancellationToken)
    {
        var result = await _orderQueryService.GetByIdDtoAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("{id:long}/projection")]
    public async Task<ActionResult<OrderDetailDto>> GetByIdProjectionAsync(long id, CancellationToken cancellationToken)
    {
        var result = await _orderQueryService.GetByIdProjectionAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}

