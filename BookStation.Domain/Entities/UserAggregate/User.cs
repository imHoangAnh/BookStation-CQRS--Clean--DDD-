using BookStation.Domain.Enums;
using BookStation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using BookStation.Core.SharedKernel;

namespace BookStation.Domain.Entities.UserAggregate;

public class User : AggregateRoot<long>
{
    public Email Email { get; private set; }
    public string FullName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Password Password { get; private set; }
    public bool IsVerified { get; private set; }
    public UserStatus Status { get; private set;
    private readonly List<UserRole> _roles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly(); 
    public SellerProfile? SellerProfile { get; private set; }

    /// <summary>
    /// // EF Core constructor
    /// </summary>
    private User() { }
    public User Create(Email email, string password, string fullname, PhoneNumber? phonenumber) 
    {
        var user = new User()
        {
            Email = email,
            FullName = fullname,
            PhoneNumber = phonenumber,
            Password = Password.Create(password),
            IsVerified = false,
            Status = UserStatus.Pending
        };
        user.AddDomainEvent(new UserCreatedEvent(user));
        return user;
    }

  

}
