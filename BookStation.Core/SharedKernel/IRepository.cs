using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BookStation.Core.SharedKernel;

public interface IRepository<TEntity, TId> where TEntity : AggregateRoot<TId> where TId : IEquatable<TId>
{

    /// <summary>
    /// Adds an entity to the repository.
    /// </summary>
    Task TaskAsync(TEntity entity, CancellationToken cancellation = default);
    
    /// <summary>
    /// Updates an entity in the repository.
    /// </summary>
    void Update(TEntity entity);
    
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>>
    void Delete(TEntity entity);

    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary> 
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellation = default);

}
