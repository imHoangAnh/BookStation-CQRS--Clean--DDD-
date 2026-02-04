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

public class CreateAddressWalletCommandHandler : IRequestHandler<CreateAddressWalletCommand, AddressWalletResult>
{
    private readonly IAddressWalletRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAddressWalletCommandHandler(IAddressWalletRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AddressWalletResult> Handle(CreateAddressWalletCommand request, CancellationToken cancellationToken)
    {
        // If set as default, remove default from other address
        if (request.IsDefault)
        {
            var currentDefault = await _addressRepository.GetDefaultByUserIdAsync(request.UserId, cancellationToken);
            if (currentDefault != null)
            {
                currentDefault.RemoveDefault();
                _addressRepository.Update(currentDefault);
            }
        }

        // Create PhoneNumber ValueObject
        var phone = PhoneNumber.Create(request.PhoneNumber);

        // Create Address ValueObject
        var address = Domain.ValueObjects.Address.Create(
            request.Street,
            request.City,
            request.Country,
            request.Ward,
            request.PostalCode
        );

        var addressWallet = DomainAddressWallet.Create(
            request.UserId,
            request.RecipientName,
            phone,
            address,
            request.Label,
            request.IsDefault
        );

        await _addressRepository.AddAsync(addressWallet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResult(addressWallet);
    }

    private static AddressWalletResult MapToResult(DomainAddressWallet addresswallet)
    {
        return new AddressWalletResult
        {
            Id = addresswallet.Id,
            RecipientName = addresswallet.RecipientName,
            PhoneNumber = addresswallet.RecipientPhone.Value,
            Street = addresswallet.RecipientAddress.Street,
            Ward = addresswallet.RecipientAddress.Ward,
            City = addresswallet.RecipientAddress.City,
            Country = addresswallet.RecipientAddress.Country,
            PostalCode = addresswallet.RecipientAddress.PostalCode,
            Label = addresswallet.Label,
            IsDefault = addresswallet.IsDefault
        };
    }
}
