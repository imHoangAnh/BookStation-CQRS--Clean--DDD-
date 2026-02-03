using System;
using MediatR;

namespace BookStation.Application.Commands.ChangePassword;

public record ChangePasswordCommand : IRequest<Unit>
{
    public required Guid UserId { get; init; }
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}
