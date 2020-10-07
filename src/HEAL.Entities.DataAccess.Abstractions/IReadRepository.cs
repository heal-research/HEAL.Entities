using HEAL.Entities.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HEAL.Entities.DataAccess.Abstractions {

  public interface IReadRepository<TEntity, TKey>
      where TEntity : IDomainObject<TKey>
      where TKey : IComparable<TKey> {
    /// <summary>
    /// counts all entries of the entity fulfilling the optional predicate
    /// </summary>
    /// <param name="filter">predicate specifying the relevant entities</param>
    long Count(Expression<Func<TEntity, bool>> filter = null);
	
    /// <summary>
    /// returns ALL entries entity
    /// </summary>
    /// <param name="filter">predicate specifying the relevant entities</param>
    /// <param name="orderBy">order clause</param>
    /// <param name="includeProperties">list of properties to be included in query</param>
    /// <returns></returns>
    IEnumerable<TEntity> GetAll();

    /// <summary>
    /// returns all entries fulfilling the optional predicate
    /// </summary>
    /// <param name="filter">predicate specifying the relevant entities</param>
    /// <param name="orderBy">order clause</param>
    /// <param name="includeProperties">list of properties to be included in query</param>
    /// <returns></returns>
    IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
    
	/// <summary>
    /// Returns the single entry matching the primary key.
    /// </summary>
    /// <param name="id">primary key</param>
    /// <returns>default of <see cref="TEntity"/> if key is not found</returns>
    TEntity GetByKey(TKey id);
  }
}
