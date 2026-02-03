using BookStation.Core.SharedKernel;
using BookStation.Domain.Enums;
using BookStation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Domain.Entities.UserAggregate;

public class AddressWallet : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string RecipientName { get; private set; } = string.Empty;
    public PhoneNumber RecipientPhone { get; private set; } = default!;
    public Address RecipientAddress { get; private set; } = default!;
    public AddressLabel Label { get; private set; } = default!;
    public bool IsDefault { get; private set; }
    public User? User { get; private set; }

    private AddressWallet() { }

    public static AddressWallet Create(Guid userId, string recipientName, PhoneNumber recipientPhone, 
                                       Address recipientAddress, AddressLabel label, bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
            throw new ArgumentException("Recipient name is required.", nameof(recipientName));

        return new AddressWallet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RecipientName = recipientName,
            RecipientPhone = recipientPhone,
            RecipientAddress = recipientAddress,
            Label = label,
            IsDefault = isDefault,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public void UpdateAddressWallet(string recipientName, PhoneNumber recipientPhone,
                                    Address recipientAddress, AddressLabel label)
    {
        RecipientName = recipientName;
        RecipientPhone = recipientPhone;
        RecipientAddress = recipientAddress;
        Label = label;
        UpdatedAt = DateTime.UtcNow;
    }
    public void SetAsDefault()
    {
        IsDefault = true;
        UpdatedAt = DateTime.UtcNow;
    }
    public void RemoveDefault()
    {
        IsDefault = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
