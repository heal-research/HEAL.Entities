using System;
using System.Collections.Generic;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {
  /// <summary>
  /// Interface for the DataVault v2 type Link that provides removal detault implementation<br></br>
  /// must be paired with <see cref="ITimelineSatellite{TLinkReference}"/> satellite to provide all functionality
  /// </summary>
  /// <typeparam name="TPrimaryKey"></typeparam>
  public interface ILinkTimeline<TPrimaryKey, TTimelineSatellite> : ILink<TPrimaryKey>
      where TPrimaryKey : IComparable<TPrimaryKey>
      where TTimelineSatellite : ITimelineSatellite<TPrimaryKey> {
    /// <summary>
    /// navigational property from Link with timeline to its timeline entries
    /// </summary>
    ICollection<TTimelineSatellite> Timeline { get; set; }

    /// <summary>
    /// returns the most recent timeline entry
    /// </summary>
    TTimelineSatellite MostRecentEntry { get; }
  }

  /// <summary>
  /// Interface for the DataVault v2 Satellite that stores timeline history of a compatible Link entry <br></br>
  /// is used in conjunction with <see cref="ILinkTimeline{TPrimaryKey, TTimelineEntry}"/> satellite to provide all functionality
  /// </summary>
  /// <typeparam name="TLinkReference"></typeparam>
  public interface ITimelineSatellite<TLinkReference> : ISatellite<TLinkReference>, IStoreEndDate
   where TLinkReference : IComparable<TLinkReference> {
  }

}
