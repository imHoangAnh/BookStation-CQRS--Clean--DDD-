using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using BookStation.Domain.Repositories;
using BookStation.Domain.ValueObjects;
using BookStation.Core.SharedKernel;

namespace BookStation.Application.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UpdateProfileResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateProfileResult> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        PhoneNumber? phoneNumber = null;
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            phoneNumber = PhoneNumber.Create(request.PhoneNumber);
        }

        user.UpdateProfile(request.FullName, phoneNumber);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateProfileResult
        {
            UserId = user.Id,
            Email = user.Email.Value,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber?.Value,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
