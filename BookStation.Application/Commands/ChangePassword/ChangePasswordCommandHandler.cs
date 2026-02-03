using System;
using System.Threading;
using System.Threading.Tasks;
using BookStation.Application.Contracts;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Repositories;
using BookStation.Domain.ValueObjects;
using MediatR;

namespace BookStation.Application.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<ChangePasswordResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        var currentPassword = Password.Create(request.CurrentPassword);
        if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            throw new InvalidOperationException("Password is incorrect.");

        var newPassword = Password.Create(request.NewPassword);
        var newPasswordHash = _passwordHasher.HashPassword(newPassword);
        user.ChangePassword(newPasswordHash);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangePasswordResult
        {
            UserId = user.Id,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
