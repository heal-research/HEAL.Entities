using HEAL.Entities.Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HEAL.Entities.DataAccess.EFCore.Extensions {
  public static class DbSetExtensions {
    public static EntityEntry<TEntity> AddIfNotExists<TEntity, TKey>(this DbSet<TEntity> dbSet
                                                                    , TEntity entity)
                        where TEntity : class, IDomainObject<TKey>
                        where TKey : IComparable<TKey> {
      return AddIfNotExists(dbSet, entity, x => x.PrimaryKey.Equals(entity.PrimaryKey));
    }

    public static EntityEntry<TEntity> AddIfNotExists<TEntity>(this DbSet<TEntity> dbSet
                                                                , TEntity entity
                                                                , Expression<Func<TEntity
                                                                , bool>> predicate = null)
            where TEntity : class {
      var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
      return !exists ? dbSet.Add(entity) : null;
    }
  }
}
