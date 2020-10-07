using System;
using System.Collections.Generic;
using System.Text;
using HEAL.Entities.Objects.Dwh.DataVaultV2;

namespace HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions{
  public interface IHubRepository<TEntity, TPrimaryKey, TReference> : IBaseRepository<TEntity, TPrimaryKey, TPrimaryKey,TReference>
      where TEntity : IDataVaultObject<TPrimaryKey>
      where TPrimaryKey : IComparable<TPrimaryKey> {
    /// <summary>
    /// calculates the <typeparamref name="TPrimaryKey"/> for the given entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    TPrimaryKey CalculateHash(TEntity entity);
  }
}
