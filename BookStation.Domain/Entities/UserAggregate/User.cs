using BookStation.Core.SharedKernel;
using BookStation.Domain.Enums;
using BookStation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BookStation.Domain.Entities.UserAggregate;

public class User : AggregateRoot<long>
{
    public Email Email { get; private set; }
    public string FullName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Password Password { get; private set; }
    public bool IsVerified { get; private set; }
    public UserStatus Status { get; private set;
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
    public User Create(Email email, string password, string fullname, PhoneNumber? phonenumber) 
    {
        var user = new User()
        {
            Email = email,
            FullName = fullname,
            PhoneNumber = phonenumber,
            Password = Password.Create(password),
            IsVerified = false, // mac dinh la chua xac thuc 
            Status = UserStatus.Pending  // mac dinh la dang cho xet duyet
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
    public void UpdateProfile(string? fullName, PhoneNumber? phonenumber)
    {
        FullName = fullName;
        PhoneNumber = phonenumber;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserUpdatedEvent(Id, nameof(UpdateProfile)));
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
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
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
    //public SellerProfile BecomeASeller(int? organizationId = null)
    //{
    //    if (SellerProfile != null)
    //        throw new InvalidOperationException("User is already a seller.");

    //    SellerProfile = SellerProfile.Create(Id, organizationId);

    //    AddDomainEvent(new UserBecameSellerEvent(Id));

    //    return SellerProfile;
    //}

    ///// <summary>
    ///// Creates a shipper profile for this user.
    ///// </summary>
    //public ShipperProfile BecomeAShipper(int organizationId)
    //{
    //    if (ShipperProfile != null)
    //        throw new InvalidOperationException("User is already a shipper.");

    //    ShipperProfile = ShipperProfile.Create(Id, organizationId);

    //    return ShipperProfile;
    //}



}
