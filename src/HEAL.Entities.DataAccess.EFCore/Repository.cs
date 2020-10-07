using HEAL.Entities.DataAccess.Abstractions;
using HEAL.Entities.Objects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HEAL.Entities.DataAccess.EFCore {
  /// <summary>
  /// derive from this class to inherit the base implementation for repository data-access to a DB
  /// </summary>
  /// <typeparam name="TEntity">Generic type of the managed domain entity</typeparam>
  /// <typeparam name="TPKey">type of the primary key</typeparam>
  public class CRUDRepository<TEntity, TKey> : ICRUDDomainRepository<TEntity, TKey>
      where TEntity : class, IDomainObject<TKey>
      where TKey : IComparable<TKey> {
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// internal context of the generic EFDomainRepository
    /// Use this property for manual Queries needed in the specific Repositories
    /// </summary>
    protected DbContext Context => _context;

    /// <summary>
    /// internal DbSet of the generic EFDomainRepository
    /// Use this property for manual Queries needed in the specific DomainObject
    /// </summary>
    protected DbSet<TEntity> DbSet => _dbSet;

    public CRUDRepository(DbContext context) {
      this._context = context;
      this._dbSet = context.Set<TEntity>();
    }

    public IEnumerable<TEntity> GetAll() {
      return Get();
    }

    public virtual IEnumerable<TEntity> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "") {
      IQueryable<TEntity> query = _dbSet;

      if (filter != null) {
        query = query.Where(filter);
      }

      foreach (var includeProperty in includeProperties.Split
          (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
        query = query.Include(includeProperty);
      }

      if (orderBy != null) {
        return orderBy(query).ToList();
      } else {
        return query.ToList();
      }
    }

    public virtual TEntity GetByKey(TKey id) {
      return _dbSet.Find(id);
    }

    public virtual TKey Insert(TEntity entity) {
      _dbSet.Add(entity);
      _context.SaveChanges();
      return entity.PrimaryKey;
    }

    public virtual void Delete(TKey id) {
      TEntity entityToDelete = _dbSet.Find(id);
      Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entityToDelete) {
      if (_context.Entry(entityToDelete).State == EntityState.Detached) {
        _dbSet.Attach(entityToDelete);
      }
      _dbSet.Remove(entityToDelete);
      _context.SaveChanges();
    }

    public virtual void Update(TEntity entityToUpdate) {
      _dbSet.Attach(entityToUpdate);
      _context.Entry(entityToUpdate).State = EntityState.Modified;
      _context.SaveChanges();
    }

    public long Count(Expression<Func<TEntity, bool>> filter = null) {
      IQueryable<TEntity> query = _dbSet;

      if (filter != null) {
        query = query.Where(filter);
      }
      return query.LongCount();
    }
  }
}
