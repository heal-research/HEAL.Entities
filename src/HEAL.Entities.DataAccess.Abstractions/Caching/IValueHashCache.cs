using System;
using System.Collections.Generic;

namespace HEAL.Entities.DataAccess.Caching.Abstractions {
  public interface IValueHashCache {
    void Clear();
    Dictionary<TPKey, THashValue> GetCachedValueHashes<TEntity, TPKey, THashValue>(Type entityType);
    void InitializeValueHashCache<TEntity, TPKey, THashValue>(Type entityType, Dictionary<TPKey, THashValue> lastStateHashes);
    void RemoveCachedValueHashes(Type entityType);
    bool TryGetCachedKeys<TEntity, TPKey, THashValue>(Type entityType, out Dictionary<TPKey, THashValue> dict);
    bool ValueHashCacheInitialized(Type entityType);
  }
}