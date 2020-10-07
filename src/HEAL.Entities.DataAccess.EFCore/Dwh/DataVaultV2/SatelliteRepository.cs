using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using HEAL.Entities.DataAccess.Caching.Abstractions;
using HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2 {

  public class SatelliteRepository<TEntity> : SatelliteRepository<TEntity, string, long>
    where TEntity : class, ISatellite<string> {

    public SatelliteRepository(DwhDbContext context
                                          , DataVaultHashFunction<string> hubKeyHashFunction
                                          , ILogger<SatelliteRepository<TEntity, string, long>> logger = null)
                            : base(context, hubKeyHashFunction, logger) {
    }
  }

  public class ValueHashedSatelliteRepository<TEntity> : SatelliteValueHashedRepository<TEntity, string, string, long>
      where TEntity : class, ISatelliteValueHashed<string, string> {

    public ValueHashedSatelliteRepository(DwhDbContext context
                                                    , DataVaultHashFunction<string> hashFunction
                                                    , DataVaultHashFunction<string> valueHashFunction
                                                    , IValueHashCache valueHashCache
                                                    , ILogger<SatelliteValueHashedRepository<TEntity, string, string, long>> logger = null)
                                       : base(context, hashFunction, valueHashFunction, valueHashCache, logger) {
    }
  }
}
