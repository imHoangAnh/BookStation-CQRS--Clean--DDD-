using BookStation.Core.SharedKernel;
using BookStation.Domain.Enums;
using BookStation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BookStation.Domain.Entities.UserAggregate;

public class User : AggregateRoot<Guid>
{


    public Email Email { get; private set; } = default!;
    public string FullName { get; private set; } = string.Empty;
    public PhoneNumber? PhoneNumber { get; private set; } 
    public PasswordHash PasswordHash { get; private set; } = default!;
    public bool IsVerified { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public Gender? Gender { get; private set; }
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }
    public SellerProfile? SellerProfile { get; private set; }
    

    /// <summary>
    /// Private EF Core constructor
    /// </summary>
    private User() { }


    /// <summary>
    /// Factory method to create a new User. 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="fullname"></param>
    /// <param name="phonenumber"></param>
    /// <returns></returns>
    public static User Create(Email email, PasswordHash password, string fullname, PhoneNumber? phonenumber) 
    {
        if (string.IsNullOrWhiteSpace(fullname))
            throw new ArgumentException("Full name is required.", nameof(fullname));

        var user = new User()
        {
            Email = email,
            FullName = fullname,
            PhoneNumber = phonenumber!, // hoặc default! nếu nullable trong entity
            PasswordHash = password,
            IsVerified = false, // mac dinh la chua xac thuc 
            Status = UserStatus.Active  // mac dinh la dang cho xet duyet
        };

        // Raise domain event để các subscriber khác xử lý
        // (Ex: gửi email chào mừng)
        user.AddDomainEvent(new UserCreatedEvent(user));
        return user;
    }

    /// Behavioral Methods

    /// <summary>
    /// Updates the user's profile information.
    /// </summary>
    public void UpdateProfile(
        string? fullName, 
        PhoneNumber? phonenumber,
        DateTime? dateOfBirth = null,
        Gender? gender = null,
        string? bio = null)
    {
        FullName = fullName ?? FullName;
        PhoneNumber = phonenumber;
        DateOfBirth = dateOfBirth ?? DateOfBirth;
        Gender = gender ?? Gender;
        Bio = bio ?? Bio;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserUpdatedEvent(Id, nameof(UpdateProfile)));
    }
    public void UpdateAvatar(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserUpdatedEvent(Id, nameof(UpdateAvatar)));
    }

    /// <summary>
    /// Updates the user's email address.
    /// </summary>
    public void UpdateEmail(Email newEmail)
    {
        if (Email == newEmail)
            return;

        var oldEmail = Email;
        Email = newEmail;
        IsVerified = false; // Require re-verification
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserEmailChangedEvent(Id, oldEmail.Value, newEmail.Value));
    }

    /// <summary>
    /// Changes the user's password.
    /// </summary>
    public void ChangePassword(PasswordHash newPasswordHash)
    {
        PasswordHash = newPasswordHash ?? throw new ArgumentException(nameof(newPasswordHash));
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserPasswordChangedEvent(Id));
    }

    /// <summary>
    /// Verifies the user's email.
    /// </summary>
    public void Verify()
    {
        if (IsVerified)
            return;

        IsVerified = true;
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserVerifiedEvent(Id));
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
            return;

        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        if (Status == UserStatus.Active)
            return;

        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Bans the user account.
    /// </summary>
    public void Ban(string reason)
    {
        Status = UserStatus.Banned;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserBannedEvent(Id, reason));
    }


    /// <summary>
    /// Creates a seller profile for this user.
    /// </summary>
    public SellerProfile BecomeASeller()
    {
        if (SellerProfile != null)
            throw new InvalidOperationException("User is already a seller.");

        SellerProfile = SellerProfile.Create(Id);

        AddDomainEvent(new UserBecameSellerEvent(Id));

        return SellerProfile;
    }
}
