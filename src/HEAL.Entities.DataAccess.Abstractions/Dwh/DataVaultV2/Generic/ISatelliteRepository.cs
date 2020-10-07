using System;
using System.Collections.Generic;
using System.Text;
using HEAL.Entities.Objects.Dwh.DataVaultV2;

namespace HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions{
  public interface ISatelliteRepository<TEntity, THubReference, TReference> : IBaseRepository<TEntity, ISatellitePrimaryKey<THubReference>, THubReference, TReference>
      where TEntity : IDataVaultObject<ISatellitePrimaryKey<THubReference>>
      where THubReference : IComparable<THubReference> {
  }
}
