using System;
using System.Linq;
using HEAL.Entities.DataAccess.Caching.Abstractions;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.Extensions.Logging;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic {
  /// <summary>
  /// derive from this class to inherit the base implementation for repository data-access to a DWH in DataVault 2.0 Schema
  /// </summary>
  /// <typeparam name="TEntity">Generic type of the managed domain entity</typeparam>
  /// <typeparam name="TPKey">type of the primary key</typeparam>
  public class SatelliteValueHashedRepository<TEntity, THubReference, TValueHash, TReference> : SatelliteRepository<TEntity, THubReference, TReference>
      where TEntity : class, ISatelliteValueHashed<THubReference, TValueHash>
      where TValueHash : IComparable<TValueHash>
      where THubReference : IComparable<THubReference> {
    public DataVaultHashFunction<TValueHash> ValueHashFunction { get; private set; }
    protected IValueHashCache ValueHashCache { get; set; }

    /// <summary>
    /// Creates a new <see cref="SatelliteValueHashedRepository{TEntity, THubReference, TValueHash,TReference}"/>
    /// </summary>
    /// <param name="context"><see cref="DwhDbContext"/> apply performance improvements per default</param>
    /// <param name="hashFunction">any hash function reducing the business-key to it's primary key</param>
    /// <param name="logger"></param>
    public SatelliteValueHashedRepository(DwhDbContext context
                                                    , DataVaultHashFunction<THubReference> hashFunction
                                                    , DataVaultHashFunction<TValueHash> valueHashFunction
                                                    , IValueHashCache valueHashCache
                                                    , ILogger<SatelliteValueHashedRepository<TEntity, THubReference, TValueHash, TReference>> logger = null)
                                              : base(context, hashFunction, logger) {
      if (valueHashFunction == null) throw new ArgumentNullException(nameof(valueHashFunction), $"Instance of {nameof(DataVaultHashFunction<TValueHash>)} must be provided.");
      ValueHashFunction = valueHashFunction;
      if (valueHashCache == null) throw new ArgumentNullException(nameof(valueHashCache), $"Instance of {nameof(IValueHashCache)} must be provided.");
      ValueHashCache = valueHashCache;

      if (!ValueHashCache.ValueHashCacheInitialized(typeof(TEntity))) {
        InitializeReferenceKeyValueHashCache();
      } else {
        Logger?.LogTrace($"'{nameof(IValueHashCache)}' for type '{typeof(TEntity)}' already initialized.");
      }
    }

    protected virtual void InitializeReferenceKeyValueHashCache() {
      var cache = GetCurrent(x => x.PrimaryKey != null).ToDictionary(d => d.PrimaryKey == null ? default : d.PrimaryKey.Reference, d => d.ValueHash);
      Logger?.LogTrace($"Initializing value hash cache. [{cache.Count}] entries found in database.");
      ValueHashCache.InitializeValueHashCache<TEntity, THubReference, TValueHash>(typeof(TEntity), cache);
    }

    protected override void DBSetAddEntry(TEntity entity) {

      entity.ValueHash = ValueHashFunction(entity.GetValueString());

      var valueHashes = ValueHashCache.GetCachedValueHashes<TEntity, THubReference, TValueHash>(typeof(TEntity));
      var key = entity.PrimaryKey.Reference;
      var value = entity.ValueHash;
      bool doInsert = false;

      TValueHash cachedValue = default(TValueHash);
      if (!valueHashes.TryGetValue(entity.PrimaryKey.Reference, out cachedValue)) {
        //no value yet present in dictionary -> can insert
        doInsert = true;
      } else if (cachedValue.CompareTo(value) != 0) {
        Logger?.LogTrace($"Reference value '{entity.PrimaryKey.Reference}' already saved to database but values have changed. Inserting current state.");

        // hub reference already seen but hash value is different
        doInsert = true;
        valueHashes.Remove(key);  //remove old value hash associated with reference key
      }

      if (doInsert) {
        DbSet.Add(entity);
        valueHashes.Add(key, value); // add new value hash to reference
      }
    }
  }
}
