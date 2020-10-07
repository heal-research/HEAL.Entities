using System;
using System.Collections.Generic;
using System.Text;
using HEAL.Entities.DataAccess.Caching.Abstractions;

namespace HEAL.Entities.DataAccess.EFCore.Caching {

  public enum SatelliteValueHashCaching {
    Disabled, Enabled
  }

  public class BaseValueHashCache : IValueHashCache {
    private Dictionary<Type, object> ValueHashes { get; set; } = new Dictionary<Type, object>();

    public void InitializeValueHashCache<TEntity, TPKey, THashValue>(Type entityType, Dictionary<TPKey, THashValue> lastStateHashes) {
      if (ValueHashCacheInitialized(entityType))
        throw new Exception($"Key Collection for type '{entityType.ToString()}' was already initialized. Hash values should not be overwritten.");
      ValueHashes.Add(entityType, lastStateHashes);
    }

    public bool ValueHashCacheInitialized(Type entityType) {
      return ValueHashes.ContainsKey(entityType);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPKey"></typeparam>
    /// <param name="entityType"></param>
    /// <param name="set"></param>
    /// <returns>true if cache was initialized and key was found</returns>
    public bool TryGetCachedKeys<TEntity, TPKey, THashValue>(Type entityType, out Dictionary<TPKey, THashValue> dict) {
      if (ValueHashes.ContainsKey(entityType)) {
        object value;
        var found = ValueHashes.TryGetValue(entityType, out value);
        dict = (Dictionary<TPKey, THashValue>)value;
        return found;
      }
      dict = new Dictionary<TPKey, THashValue>();
      return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPKey"></typeparam>
    /// <typeparam name="THashValue"></typeparam>
    /// <param name="entityType"></param>
    /// <exception cref="KeyNotFoundException">If Value Hash Cache was not initialized for the given type</exception>
    /// <returns></returns>
    public Dictionary<TPKey, THashValue> GetCachedValueHashes<TEntity, TPKey, THashValue>(Type entityType) {
      if (ValueHashes.ContainsKey(entityType)) {
        object value;
        return ValueHashes.TryGetValue(entityType, out value) == true ? (Dictionary<TPKey, THashValue>)value : (Dictionary<TPKey, THashValue>)null;
      }
      throw new KeyNotFoundException($"Key Collection for type '{entityType.ToString()}' was not initialized");
    }

    public void RemoveCachedValueHashes(Type entityType) {
      if (ValueHashes.ContainsKey(entityType)) {
        ValueHashes.Remove(entityType);
      }
    }

    public void Clear() {
      ValueHashes = new Dictionary<Type, object>();
    }
  }

}
