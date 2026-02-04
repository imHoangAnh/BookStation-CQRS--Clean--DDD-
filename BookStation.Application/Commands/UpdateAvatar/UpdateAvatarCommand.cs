using System;
using MediatR;

namespace BookStation.Application.Commands.UpdateAvatar;

public record UpdateAvatarCommand : IRequest<UpdateAvatarResult>
{
    public required Guid UserId { get; init; }
    public required string AvatarUrl { get; init; }
}

public record UpdateAvatarResult
{
    public Guid UserId { get; init; }
    public required string AvatarUrl { get; init; }
    public DateTime UpdatedAt { get; init; }
}
