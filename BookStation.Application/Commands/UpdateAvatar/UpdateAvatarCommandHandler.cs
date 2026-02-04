using BookStation.Application.Commands.UpdateAvatar;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class UpdateAvatarCommandHandler : IRequestHandler<UpdateAvatarCommand, UpdateAvatarResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAvatarCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateAvatarResult> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        user.UpdateAvatar(request.AvatarUrl);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateAvatarResult
        {
            UserId = user.Id,
            AvatarUrl = user.AvatarUrl!,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
