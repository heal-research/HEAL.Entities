using System;
using System.Collections.Generic;
using System.Text;
using HEAL.Entities.DataAccess.Caching.Abstractions;

namespace HEAL.Entities.DataAccess.EFCore.Caching {

  public enum DVKeyCaching {
    Disabled, Enabled
  }

  public class BasePrimaryKeyCache : IPrimaryKeyCache {
    private Dictionary<Type, object> PrimaryKeys { get; set; } = new Dictionary<Type, object>();

    public void InitializeKeyCache<TEntity, TPKey>(Type entityType, HashSet<TPKey> keys) {
      if (KeyCacheInitialized(entityType))
        throw new Exception($"Key Collection for type '{entityType.ToString()}' was already initialized. Keys should not be overwritten.");
      PrimaryKeys.Add(entityType, keys);
    }

    public bool KeyCacheInitialized(Type entityType) {
      return PrimaryKeys.ContainsKey(entityType);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPKey"></typeparam>
    /// <param name="entityType"></param>
    /// <exception cref="KeyNotFoundException">If Key Cache was not initialized for the given type</exception>
    /// <returns></returns>
    public HashSet<TPKey> GetCachedKeys<TEntity, TPKey>(Type entityType) {
      if (PrimaryKeys.ContainsKey(entityType)) {
        object value;
        return PrimaryKeys.TryGetValue(entityType, out value) == true ? (HashSet<TPKey>)value : (HashSet<TPKey>)null;
      }
      throw new KeyNotFoundException($"Key Collection for type '{entityType.ToString()}' was not initialized");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPKey"></typeparam>
    /// <param name="entityType"></param>
    /// <param name="set"></param>
    /// <returns>true if cache was initialized and key was found</returns>
    public bool TryGetCachedKeys<TEntity, TPKey>(Type entityType, out HashSet<TPKey> set) {
      if (PrimaryKeys.ContainsKey(entityType)) {
        object value;
        var found = PrimaryKeys.TryGetValue(entityType, out value);
        set = (HashSet<TPKey>)value;
        return found;
      }
      set = new HashSet<TPKey>();
      return false;
    }

    public void RemoveCachedKeys(Type entityType) {
      if (PrimaryKeys.ContainsKey(entityType)) {
        PrimaryKeys.Remove(entityType);
      }
    }
    public void Clear() {
      PrimaryKeys = new Dictionary<Type, object>();
    }
  }
}
