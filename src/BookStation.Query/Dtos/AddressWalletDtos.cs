using BookStation.Core.Enums;

namespace BookStation.Query.Dtos;

public record AddressWalletDto
{
    public Guid Id { get; init; }
    public string RecipientName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string Ward { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? PostalCode { get; init; }
    public AddressLabel Label { get; init; }
    public bool IsDefault { get; init; }
}
