using HEAL.Entities.DataAccess.Dwh.DataVaultV2.Abstractions;
using HEAL.Entities.DataAccess.Caching.Abstractions;
using HEAL.Entities.DataAccess.EFCore.Caching;
using HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.Extensions.Logging;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2 {

  public class LinkRepository<TEntity> : LinkRepository<TEntity, string,long>, ILinkRepository<TEntity>
    where TEntity : class, ILink<string> {

    public LinkRepository(DwhDbContext context
                                    , DataVaultHashFunction<string> hashFunction
                                  , IPrimaryKeyCache keyCache = null
                                  , DVKeyCaching useKeyCaching = DVKeyCaching.Disabled
                                    , ILogger<LinkRepository<TEntity, string,long>> logger = null) 
                    : base(context, hashFunction,keyCache,useKeyCaching,logger) {
    }

  }
}