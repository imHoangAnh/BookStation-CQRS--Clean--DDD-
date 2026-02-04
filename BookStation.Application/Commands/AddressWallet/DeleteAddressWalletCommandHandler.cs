using BookStation.Application.Commands.Address;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookStation.Application.Commands.AddressWallet;

public class DeleteAddressWalletCommandHandler : IRequestHandler<DeleteAddressWalletCommand>
{
    private readonly IAddressWalletRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAddressWalletCommandHandler(IAddressWalletRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(DeleteAddressWalletCommand request, CancellationToken cancellationToken)
    {
        var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);

        if (address == null)
            throw new InvalidOperationException("Address not found.");

        if (address.UserId != request.UserId)
            throw new UnauthorizedAccessException("You do not have permission to delete this address.");

        _addressRepository.Delete(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
