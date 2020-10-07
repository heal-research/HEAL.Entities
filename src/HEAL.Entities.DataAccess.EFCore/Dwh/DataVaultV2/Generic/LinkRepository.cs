using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HEAL.Entities.DataAccess.Caching.Abstractions;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions;
using HEAL.Entities.DataAccess.EFCore.Caching;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic {
  /// <summary>
  /// derive from this class to inherit the base implementation for repository data-access to a DWH in DataVault 2.0 Schema
  /// </summary>
  /// <typeparam name="TEntity">Generic type of the managed domain entity</typeparam>
  /// <typeparam name="TPKey">type of the primary key</typeparam>
  public abstract class LinkRepository<TEntity, TPrimaryKey, TReference> : BaseRepository<TEntity, TPrimaryKey, TPrimaryKey, TReference>, ILinkRepository<TEntity, TPrimaryKey, TReference>
      where TEntity : class, ILink<TPrimaryKey>
      where TPrimaryKey : IComparable<TPrimaryKey> {

    public DVKeyCaching UseKeyCaching { get; private set; }

    protected IPrimaryKeyCache KeyCache { get; set; }

    /// <summary>
    /// Creates a new <see cref="LinkRepository{TEntity, TPrimaryKey}"/>
    /// </summary>
    /// <param name="context"><see cref="DwhDbContext"/> apply performance improvements per default</param>
    /// <param name="hashFunction">any hash function reducing the hubs business-key to it's primary key</param>
    /// <param name="logger"></param>
    public LinkRepository(DwhDbContext context
                                  , DataVaultHashFunction<TPrimaryKey> hashFunction
                                  , IPrimaryKeyCache keyCache = null
                                  , DVKeyCaching useKeyCaching = DVKeyCaching.Disabled
                                  , ILogger<LinkRepository<TEntity, TPrimaryKey, TReference>> logger = null)
                              : base(context, hashFunction,logger) {
      UseKeyCaching = useKeyCaching;
      if (UseKeyCaching == DVKeyCaching.Enabled) {
        if (keyCache == null) throw new ArgumentNullException(nameof(keyCache), $"Must provide instance of {nameof(IPrimaryKeyCache)} if parameter '{nameof(useKeyCaching)}' is not {DVKeyCaching.Disabled}");
        KeyCache = keyCache;
        if (!KeyCache.KeyCacheInitialized(typeof(TEntity))) {
          InitializePrimaryKeyCache();
        } else {
          Logger?.LogTrace($"'{nameof(IPrimaryKeyCache)}' for type '{typeof(TEntity)}' already initialized.");
        }
      } else {
        Logger?.LogWarning($"Primary key caching was not activated. This can result in Unique-Key Constraint violations on runtime. Consider configuring '{nameof(DVKeyCaching)}{nameof(DVKeyCaching.Enabled)}'.");
      }
    }

    public override TEntity CalculateHashes(TEntity entity) {
      entity.PrimaryKey = HashFunction(entity.GetBusinessKeyString());
      return entity;
    }

    protected override void DBSetAddEntry(TEntity entity) {
      if (UseKeyCaching == DVKeyCaching.Enabled) {
        if (AddIfKeyIsUnique<TEntity>(entity.PrimaryKey)) {
          DbSet.Add(entity);
        } else {
          Logger?.LogTrace($"Primary key '{entity.PrimaryKey}' was already contained in '{nameof(IPrimaryKeyCache)}'. Refraining from DB insert.");
        }
      } else { DbSet.Add(entity); }
    }

    protected bool AddIfKeyIsUnique<T>(TPrimaryKey primaryKey) {
      var PrimaryKeys = KeyCache.GetCachedKeys<T, TPrimaryKey>(typeof(T));
      return PrimaryKeys.Add(primaryKey);
    }

    public override IEnumerable<TEntity> GetCurrent(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "") {
      IQueryable<TEntity> query = DbSet;

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

    protected virtual void InitializePrimaryKeyCache() {
      var cache = new HashSet<TPrimaryKey>(DbSet.Select(x => x.PrimaryKey).Distinct());
      Logger?.LogTrace($"Initializing value hash cache. [{cache.Count}] entries found in database.");
      KeyCache.InitializeKeyCache<TEntity, TPrimaryKey>(typeof(TEntity), cache);
    }
  }

}