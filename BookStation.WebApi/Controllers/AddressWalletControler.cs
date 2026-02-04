using BookStation.Application.Commands.Address;
using BookStation.Application.Queries.AddressWallet;
using BookStation.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStation.WebApi.Controllers;

/// <summary>
/// Address controller for managing user's address wallet.
/// </summary>
[ApiController]
[Route("address")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IMediator _mediator;

    public AddressController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all addresses for the current user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AddressWalletResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllAddresses()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _mediator.Send(new GetAllAddressesQuery(userId.Value));
        return Ok(result);
    }

    /// <summary>
    /// Create a new address.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AddressWalletResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        try
        {
            var command = new CreateAddressWalletCommand
            {
                UserId = userId.Value,
                RecipientName = request.RecipientName,
                PhoneNumber = request.RecipientPhone,
                Street = request.Street,
                Ward = request.Ward,
                City = request.City,
                Country = request.Country ?? "Vietnam",
                PostalCode = request.PostalCode,
                Label = request.Label,
                IsDefault = request.IsDefault
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAllAddresses), new { }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing address.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AddressWalletResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        try
        {
            var command = new UpdateAddressWalletCommand
            {
                AddressId = id,
                UserId = userId.Value,
                RecipientName = request.RecipientName,
                PhoneNumber = request.RecipientPhone,
                Street = request.Street,
                Ward = request.Ward,
                City = request.City,
                Country = request.Country ?? "Vietnam",
                PostalCode = request.PostalCode,
                Label = request.Label,
                IsDefault = request.IsDefault
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete an address.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        try
        {
            await _mediator.Send(new DeleteAddressWalletCommand { AddressId = id, UserId = userId.Value });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;
        return userId;
    }
}

// Request DTOs
public record CreateAddressRequest(
    string RecipientName,
    string RecipientPhone,
    string Street,
    string Ward,
    string City,
    string? Country,
    string? PostalCode,
    AddressLabel Label,
    bool IsDefault = false
);

public record UpdateAddressRequest(
    string RecipientName,
    string RecipientPhone,
    string Street,
    string Ward,
    string City,
    string? Country,
    string? PostalCode,
    AddressLabel Label,
    bool IsDefault = false
);
