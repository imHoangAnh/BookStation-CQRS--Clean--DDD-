using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BookStation.Application.Commands.UpdateAvatar;

public record UpdateAvatarCommand : IRequest<string>
{
    public required Guid UserId { get; init; }
    public required string AvatarUrl { get; init; }
}
