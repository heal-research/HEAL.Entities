using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using HEAL.Entities.Objects.Dwh.DataVaultV2;

namespace HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions{
  public interface ILinkRepository<TLink, TPrimaryKey, TReference> : IBaseRepository<TLink, TPrimaryKey, TPrimaryKey, TReference>
      where TLink : IDataVaultObject<TPrimaryKey>
      where TPrimaryKey : IComparable<TPrimaryKey> {
  }

  public interface ILinkTimelineRepository<TLink, TPrimaryKey, TSatelliteTimeline, TReference> : ILinkRepository<TLink, TPrimaryKey, TReference>
        where TLink : ILinkTimeline<TPrimaryKey, TSatelliteTimeline>
        where TSatelliteTimeline : ITimelineSatellite<TPrimaryKey>
        where TPrimaryKey : IComparable<TPrimaryKey> {

    /// <summary>
    /// Link is no longer valid, updates the latest entry of <see cref="TSatelliteTimeline"/> and sets its enddate
    /// </summary>
    /// <param name="entity">the key of the link entry that should be removed</param>
    /// <param name="removalDate">timestamp from which on the link is invalid</param>
    void RemoveLink(TLink entity, DateTime removalDate);

    /// <summary>
    /// calls <see cref="RemoveLink(TEntity, DateTime, string)"/> for every entry but SaveChanges only once
    /// </summary>
    /// <param name="entities">list of links that should be removed</param>
    /// <param name="removalDate">timestamp from which on the link is invalid</param>
    void RemoveLink(IEnumerable<TLink> entities, DateTime removalDate);

  }


}
