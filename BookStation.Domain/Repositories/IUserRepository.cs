using System;
using System.Collections.Generic;
using System.Text;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Entities.UserAggregate;

namespace BookStation.Domain.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    /// <summary>
    /// Gets user by email.
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellation = default);

    /// <summary>
    /// Checks if a user with the given email exists.
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellation = default);

    /// <summary>
    /// Gets user with their roles.
    /// </summary>  
    Task<User?> GetWithRolesAsync(Guid id, CancellationToken cancellation = default);

    /// <summary>
    /// Gets user with their seller profile.
    /// </summary>
    Task<User?> GetWithSellerProfileAsync(Guid id, CancellationToken cancellation = default);
}
