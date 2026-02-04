using BookStation.Application.Commands.Address;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Repositories;
using BookStation.Domain.ValueObjects;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using DomainAddressWallet = BookStation.Domain.Entities.UserAggregate.AddressWallet;

namespace BookStation.Application.Commands.AddressWallet;

public class UpdateAddressWalletCommandHandler : IRequestHandler<UpdateAddressWalletCommand, AddressWalletResult>
{
    private readonly IAddressWalletRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAddressWalletCommandHandler(IAddressWalletRepository addressRepository, IUnitOfWork unitOfWork)
    {   
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<AddressWalletResult> Handle(UpdateAddressWalletCommand request, CancellationToken cancellationToken)
    {
        var addressWallet = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);

        if (addressWallet == null)
            throw new InvalidOperationException("Address not found.");

        if (addressWallet.UserId != request.UserId)
            throw new UnauthorizedAccessException("You do not have permission to update this address.");

        // If setting as default, remove default from other addresses
        if (request.IsDefault && !addressWallet.IsDefault)
        {
            var currentDefault = await _addressRepository.GetDefaultByUserIdAsync(request.UserId, cancellationToken);
            if (currentDefault != null && currentDefault.Id != addressWallet.Id)
            {
                currentDefault.RemoveDefault();
                _addressRepository.Update(currentDefault);
            }
            addressWallet.SetAsDefault();
        }
        else if (!request.IsDefault && addressWallet.IsDefault)
        {
            addressWallet.RemoveDefault();
        }

        // Create PhoneNumber ValueObject (with validation)
        var phone = PhoneNumber.Create(request.PhoneNumber);

        // Create new Address ValueObject
        var address = Domain.ValueObjects.Address.Create(
            request.Street,
            request.City,
            request.Country,
            request.Ward,
            request.PostalCode
        );

        addressWallet.UpdateAddressWallet(
            request.RecipientName,
            phone,
            address,
            request.Label
        );

        _addressRepository.Update(addressWallet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResult(addressWallet);
    }

    private static AddressWalletResult MapToResult(DomainAddressWallet wallet)
    {
        return new AddressWalletResult
        {
            Id = wallet.Id,
            RecipientName = wallet.RecipientName,
            PhoneNumber = wallet.RecipientPhone.Value,
            Street = wallet.RecipientAddress.Street,
            Ward = wallet.RecipientAddress.Ward,
            City = wallet.RecipientAddress.City,
            Country = wallet.RecipientAddress.Country,
            PostalCode = wallet.RecipientAddress.PostalCode,
            Label = wallet.Label,
            IsDefault = wallet.IsDefault
        };
    }
}
