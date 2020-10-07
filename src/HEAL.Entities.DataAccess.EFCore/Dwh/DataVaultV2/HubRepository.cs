using HEAL.Entities.DataAcces.Dwh.DataVaultV2.Abstractions;
using HEAL.Entities.DataAccess.Caching.Abstractions;
using HEAL.Entities.DataAccess.EFCore.Caching;
using HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.Extensions.Logging;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2 {

  public class HubRepository<TEntity> : HubRepository<TEntity, string,long>, IHubRepository<TEntity>
       where TEntity : class, IHub<string> {

    public HubRepository(DwhDbContext context
                                    , DataVaultHashFunction<string> hashFunction
                                    , IPrimaryKeyCache keyCache = null
                                    , DVKeyCaching useKeyCaching = DVKeyCaching.Disabled
                                    , ILogger<HubRepository<TEntity, string, long>> logger = null)
                      : base(context, hashFunction, keyCache, useKeyCaching,logger) {
    }
  }
}
