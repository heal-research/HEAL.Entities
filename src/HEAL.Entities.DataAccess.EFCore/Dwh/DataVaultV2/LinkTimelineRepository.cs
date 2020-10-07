using HEAL.Entities.DataAccess.Caching.Abstractions;
using HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using Microsoft.Extensions.Logging;
using HEAL.Entities.DataAccess.EFCore.Caching;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2 {



  public class LinkTimelineRepository<TLink, TSatelliteTimeline> : LinkTimelineRepository<TLink, string, TSatelliteTimeline, long>
                  where TLink : class, ILinkTimeline<string, TSatelliteTimeline>
                  where TSatelliteTimeline : class, ITimelineSatellite<string>, new() {

    public LinkTimelineRepository(DwhDbContext context
                                    , DataVaultHashFunction<string> hashFunction
                                    , IPrimaryKeyCache keyCache = null
                                    , DVKeyCaching useKeyCaching = DVKeyCaching.Disabled
                                    , ILogger<LinkTimelineRepository<TLink, TSatelliteTimeline>> logger = null)
                    : base(context, hashFunction, keyCache, useKeyCaching, logger) {
    }
  }
}