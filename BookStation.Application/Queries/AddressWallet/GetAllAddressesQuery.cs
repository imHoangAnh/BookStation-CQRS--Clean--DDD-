using BookStation.Application.Commands.Address;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookStation.Application.Queries.AddressWallet;

public record GetAllAddressesQuery(Guid UserId) : IRequest<List<AddressWalletResult>>;

public class GetAllAddressesQueryHandler : IRequestHandler<GetAllAddressesQuery, List<AddressWalletResult>>
{
    private readonly IAddressWalletRepository _addressRepository;

    public GetAllAddressesQueryHandler(IAddressWalletRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<List<AddressWalletResult>> Handle(GetAllAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await _addressRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);

        var results = new List<AddressWalletResult>();
        foreach (var address in addresses)
        {
            results.Add(new AddressWalletResult
            {
                Id = address.Id,
                RecipientName = address.RecipientName,
                PhoneNumber = address.RecipientPhone.Value,
                Street = address.RecipientAddress.Street,
                Ward = address.RecipientAddress.Ward,
                City = address.RecipientAddress.City,
                Country = address.RecipientAddress.Country,
                PostalCode = address.RecipientAddress.PostalCode,
                Label = address.Label,
                IsDefault = address.IsDefault
            });
        }

        return results;
    }
}
