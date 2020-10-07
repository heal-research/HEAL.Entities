using System;
using System.Collections.Generic;

namespace HEAL.Entities.DataAccess.Caching.Abstractions {
  public interface IPrimaryKeyCache {
    void Clear();
    HashSet<TPKey> GetCachedKeys<TEntity, TPKey>(Type entityType);
    void InitializeKeyCache<TEntity, TPKey>(Type entityType, HashSet<TPKey> keys);
    bool KeyCacheInitialized(Type entityType);
    void RemoveCachedKeys(Type entityType);
    bool TryGetCachedKeys<TEntity, TPKey>(Type entityType, out HashSet<TPKey> set);
  }
}