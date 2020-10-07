using HEAL.Entities.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HEAL.Entities.DataAccess.Abstractions {

  /// <summary>
  /// Extends the <see cref="IReadRepository{TEntity, TKey}"/> by create, update and delete method definitions
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  /// <typeparam name="TKey"></typeparam>
  public interface ICRUDDomainRepository<TEntity, TKey> : IReadRepository<TEntity, TKey>
      where TEntity : IDomainObject<TKey>
      where TKey : IComparable<TKey> {

    /// <summary>
    /// Persists a given entity in the storage
    /// </summary>
    /// <param name="entity">the entity to store</param>
    /// <returns>key of the entity</returns>
    TKey Insert(TEntity entity);
    /// <summary>
    /// Deletes a given entity from storage
    /// </summary>
    /// <param name="entityToDelete">entity to delete</param>
    void Delete(TEntity entityToDelete);
    /// <summary>
    /// Deletes entities with a certain key
    /// </summary>
    /// <param name="id">id of entities to delete</param>
    void Delete(TKey id);
    /// <summary>
    /// updates an already stored entity with the values provided by the argument
    /// </summary>
    /// <param name="entityToUpdate">new values to be stored</param>
    void Update(TEntity entityToUpdate);
  }
}
