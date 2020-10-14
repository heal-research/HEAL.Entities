using HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions;
using HEAL.Entities.Objects.Dwh.DataVaultV2;

namespace HEAL.Entities.DataAccess.Dwh.DataVaultV2.Abstractions {

  public interface IHubRepository<TEntity> : IHubRepository<TEntity, string,long>
      where TEntity : IDataVaultObject<string> {

  }
}
