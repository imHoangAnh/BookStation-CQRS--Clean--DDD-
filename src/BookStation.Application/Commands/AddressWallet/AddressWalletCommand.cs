using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BookStation.Core.SharedKernel;
using BookStation.Core.Entities.UserAggregate;
using BookStation.Core.Repositories;
using BookStation.Core.ValueObjects;
using BookStation.Core.Enums;
using MediatR;

namespace BookStation.Application.Commands.Address;

public record CreateAddressWalletCommand(
    Guid UserId,
    string RecipientName,
    string PhoneNumber,
    string Street,
    string Ward,
    string City,
    string Country,
    string? PostalCode,
    AddressLabel Label,
    bool IsDefault
    ) : IRequest<AddressWalletResult>;
public record UpdateAddressWalletCommand : IRequest<AddressWalletResult>
{
    public required Guid AddressId { get; init; }
    public required Guid UserId { get; init; }
    public required string RecipientName { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Street { get; init; }
    public required string Ward { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public string? PostalCode { get; init; }
    public AddressLabel Label { get; init; }
    public bool IsDefault { get; init; } = true;
}
public record DeleteAddressWalletCommand : IRequest
{
    public required Guid AddressId { get; init; }
    public required Guid UserId { get; init; }
}
public record AddressWalletResult
{
    public Guid Id { get; init; }
    public required string RecipientName { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Street { get; init; }
    public required string Ward { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public string? PostalCode { get; init; }
    public AddressLabel Label { get; init; }
    public bool IsDefault { get; init; }
}


/// <summary>
// Vì frontend thường:

//Update xong muốn render lại

//Không muốn gọi lại GET
// Nên sau khi tạo/sửa/xóa xong sẽ trả về luôn object mới để frontend render lại