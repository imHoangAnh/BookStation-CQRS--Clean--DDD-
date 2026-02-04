using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BookStation.Core.SharedKernel;
using BookStation.Domain.Entities.UserAggregate;
using BookStation.Domain.Repositories;
using BookStation.Domain.ValueObjects;
using BookStation.Domain.Enums;
using MediatR;

namespace BookStation.Application.Commands.Address;

public record CreateAddressWalletCommand : IRequest<AddressWalletResult>
{
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

