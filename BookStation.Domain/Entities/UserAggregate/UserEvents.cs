using BookStation.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStation.Domain.Entities.UserAggregate;

/// <summary>
/// Base event cho các sự kiện liên quan đến User.
/// </summary>
public abstract class UserEvents : DomainEvent
{
    public long UserId { get; }
    protected UserEvents(long userId)
    {
        UserId = userId;
    }
}
/// <summary>
/// Event when a new user is created.
/// </summary>
public sealed class UserCreatedEvent : UserEvents
{
    public string Email { get; }

    public UserCreatedEvent(User user) : base(user.Id)
    {
        Email = user.Email.Value;
    }
}

/// <summary>
/// Event when a user is updated.
/// </summary>
public sealed class UserUpdatedEvent : UserEvents
{
    public string UpdateType { get; }

    public UserUpdatedEvent(long userId, string updateType) : base(userId)
    {
        UpdateType = updateType;
    }
}

/// <summary>
/// Event when a user's email is changed.
/// </summary>
public sealed class UserEmailChangedEvent : UserEvents
{
    public string OldEmail { get; }
    public string NewEmail { get; }

    public UserEmailChangedEvent(long userId, string oldEmail, string newEmail) : base(userId)
    {
        OldEmail = oldEmail;
        NewEmail = newEmail;
    }
}

/// <summary>
/// Event when a user's password is changed.
/// </summary>
public sealed class UserPasswordChangedEvent : UserEvents
{
    public UserPasswordChangedEvent(long userId) : base(userId)
    {
    }
}

/// <summary>
/// Event when a user is verified.
/// </summary>
public sealed class UserVerifiedEvent : UserEvents
{
    public UserVerifiedEvent(long userId) : base(userId)
    {
    }
}

/// <summary>
/// Event when a user is deactivated.
/// </summary>
public sealed class UserDeactivatedEvent : UserEvents
{
    public UserDeactivatedEvent(long userId) : base(userId)
    {
    }
}

/// <summary>
/// Event when a user is banned.
/// </summary>
public sealed class UserBannedEvent : UserEvents
{
    public string Reason { get; }

    public UserBannedEvent(long userId, string reason) : base(userId)
    {
        Reason = reason;
    }
}

/// <summary>
/// Event when a user becomes a seller.
/// </summary>
public sealed class UserBecameSellerEvent : UserEvents
{
    public UserBecameSellerEvent(long userId) : base(userId)
    {
    }
}
