using HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions;
using HEAL.Entities.Objects.Dwh.DataVaultV2;

namespace HEAL.Entities.DataAccess.Dwh.DataVaultV2.Abstractions {

  public interface ILinkRepository<TEntity> : ILinkRepository<TEntity, string,long>
      where TEntity : IDataVaultObject<string> {

  }

  public interface ILinkTimelineRepository<TLink, TSatelliteTimeline> : ILinkTimelineRepository<TLink, string, TSatelliteTimeline, long>
      where TLink : ILinkTimeline<string, TSatelliteTimeline>
      where TSatelliteTimeline : ITimelineSatellite<string>{

  }
}
