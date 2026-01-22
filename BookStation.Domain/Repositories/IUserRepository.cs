using System;
using System.Collections.Generic;
using System.Text;
using BookStation.Core.SharedKernel;
using BookStation.Domain.Entities.UserAggregate;
using BookStation.Domain.UserAggregate;

namespace BookStation.Domain.Repositories;

public interface IUserRepository : IRepository<User, long>
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
    Task<User?> GetWithRolesAsync(long id, CancellationToken cancellation = default);

    /// <summary>
    /// Gets user with their seller profile.
    /// </summary>
    Task<User?> GetWithSellerProfileAsync(long id, CancellationToken cancellation = default);
}
