using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Entities.DataAccess.Caching.Abstractions;
using HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions;
using HEAL.Entities.DataAccess.EFCore.Caching;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.Extensions.Logging;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic {
  /// <summary>
  /// derive from this class to inherit the base implementation for repository data-access to a DWH in DataVault 2.0 Schema.
  /// Activate <see cref="DVKeyCaching"/> by providing the Enabled value in the constructur<see cref="HubRepository{TEntity, TPrimaryKey}.EFHubDataVaultRepository(DwhDbContext, DataVaultHashFunction{THubPKey}, DVKeyCaching)"/>
  /// </summary>
  /// <typeparam name="TEntity">Generic type of the managed domain entity</typeparam>
  /// <typeparam name="TPrimaryKey">type of the primary key</typeparam>
  public class HubRepository<TEntity, TPrimaryKey, TReference> : BaseRepository<TEntity, TPrimaryKey, TPrimaryKey, TReference>, IHubRepository<TEntity, TPrimaryKey, TReference>
      where TEntity : class, IHub<TPrimaryKey>
      where TPrimaryKey : IComparable<TPrimaryKey> {
    public DVKeyCaching UseKeyCaching { get; private set; }
    protected IPrimaryKeyCache KeyCache { get; set; }

    /// <summary>
    /// Creates a new <see cref="HubRepository{TEntity, TPrimaryKey}"/>
    /// </summary>
    /// <param name="context"><see cref="DwhDbContext"/> apply performance improvements per default</param>
    /// <param name="hashFunction">any hash function reducing the hubs business-key to it's primary key</param>
    /// <param name="logger"></param>
    /// <param name="useKeyCaching">Enables or Disables (Default) the caching if primary keys -> entrie does not get added to db set if the key was seen before. Cache is built up on creation of the repo. </param>
    public HubRepository(DwhDbContext context
                                    , DataVaultHashFunction<TPrimaryKey> hashFunction
                                    , IPrimaryKeyCache keyCache = null
                                    , DVKeyCaching useKeyCaching = DVKeyCaching.Disabled
                                    , ILogger<HubRepository<TEntity, TPrimaryKey, TReference>> logger = null)
                                : base(context, hashFunction,logger) {
      Logger = logger;
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


    public TPrimaryKey CalculateHash(TEntity entity) {
      return HashFunction(entity.GetBusinessKeyString());
    }
    public override TEntity CalculateHashes(TEntity entity) {
      entity.PrimaryKey = HashFunction(entity.GetBusinessKeyString());
      return entity;
    }

    protected override void DBSetAddEntry(TEntity entity) {
      if (UseKeyCaching == DVKeyCaching.Enabled) {
        var PrimaryKeys = KeyCache.GetCachedKeys<TEntity, TPrimaryKey>(typeof(TEntity));
        if (PrimaryKeys.Add(entity.PrimaryKey)) {
          DbSet.Add(entity);
        } else {
          Logger?.LogTrace($"Primary key '{entity.PrimaryKey}' was already added to cache. Refraining from DB insert.");
        }
      } else {
        DbSet.Add(entity);
      }
    }

    protected virtual void InitializePrimaryKeyCache() {
      var cache = new HashSet<TPrimaryKey>(DbSet.Select(x => x.PrimaryKey).Distinct());
      Logger?.LogTrace($"Initializing value hash cache. [{cache.Count}] entries found in database.");
      KeyCache.InitializeKeyCache<TEntity, TPrimaryKey>(typeof(TEntity), cache);
    }
  }
}
