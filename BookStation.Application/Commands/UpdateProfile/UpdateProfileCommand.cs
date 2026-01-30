using System;
using MediatR;

namespace BookStation.Application.Commands.UpdateProfile;

public record UpdateProfileCommand : IRequest<UpdateProfileResult>
{
    public Guid UserId { get; init; }
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }

    public UpdateProfileCommand(Guid userId, string? fullName, string? phoneNumber)
    {
        UserId = userId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
    }
}

public record UpdateProfileResult
{
    public Guid UserId { get; init; }
    public required string Email { get; init; }
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime UpdatedAt { get; init; }
}
