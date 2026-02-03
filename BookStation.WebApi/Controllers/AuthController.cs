using BookStation.Application.Commands.ChangePassword;
using BookStation.Application.Commands.Login;
using BookStation.Application.Commands.Register;
using BookStation.Application.Commands.UpdateAvatar;
using BookStation.Application.Commands.UpdateProfile;
using BookStation.Infrastructure.Services;
using BookStation.Domain.Entities.UserAggregate;
using BookStation.Domain.Enums;
using BookStation.Domain.ValueObjects;
using BookStation.Application.Contracts;
using CloudinaryDotNet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookStation.WebApi.Controllers;


/// <summary>
/// Authentication controller for all users.
/// Handles registration, login, profile management and password change.
/// </summary>
[ApiController]
[Route("")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICloudinaryService _cloudinaryService;

    public AuthController(IMediator mediator, ICloudinaryService cloudinaryService)
    {
        _mediator = mediator;
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProfile), new { }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Login user and get JWT token.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get current user profile (requires authentication).
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = User.FindFirst("FullName")?.Value;
        var phone = User.FindFirst("Phone")?.Value;

        return Ok(new
        {
            userId,
            email,
            fullName,
            phone
        });
    }

    /// <summary>
    /// Update current user profile.
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UpdateProfileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            var command = new UpdateProfileCommand(
                userId,
                request.FullName,
                request.Phone,
                request.DateOfBirth,
                request.Gender,
                request.Bio
            );
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Change current user password.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        if (request.NewPassword != request.ConfirmPassword)
            return BadRequest(new { error = "New password and confirm password do not match." });

        try
        {
            var command = new ChangePasswordCommand
            {
                UserId = userId,
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            };
            await _mediator.Send(command);
            return Ok(new { message = "Password changed successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message }); // Incorrect password
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    /// <summary>
    /// Upload avatar for current user.
    /// </summary>
    [HttpPost("update-avatar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadAvatar([FromForm] UpdateAvatarRequest request)
    {
        var file = request.File;
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "File is empty." });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            // Upload to Cloudinary
            var avatarUrl = await _cloudinaryService.UploadImageAsync(file);

            var command = new UpdateAvatarCommand
            { 
                UserId = userId,
                AvatarUrl = avatarUrl
            }; 
            // Update user profile
            await _mediator.Send(command);

            return Ok(new { avatarUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while uploading file.", details = ex.Message });
        }
    }



}

// Request DTOs
public record UpdateProfileRequest(
    string? FullName, 
    string? Phone, 
    DateTime? DateOfBirth = null, 
    Gender? Gender = null, 
    string? Bio = null
);
public record ChangePasswordRequest(
    string CurrentPassword, 
    string NewPassword, 
    string ConfirmPassword
);
public class UpdateAvatarRequest { 
    public IFormFile File { get; set; } = null!;
};










































// tai sao login va register khong can DTO
//Không cần userId - Đây là các endpoint anonymous, không cần thông tin user từ Claims
//Data hoàn chỉnh từ client - Client gửi đủ thông tin(email, password, etc.) 
//Đơn giản hơn - Không cần layer chuyển đổi trung gian