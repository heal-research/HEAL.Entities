namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {
  /// <summary>
  /// default interface for the DataVault v2 type Link that provides removal detault implementation<br></br>
  /// must be paired with <see cref="ITimelineSatellite"/> satellite to provide all functionality
  /// </summary>
  /// <typeparam name="TPrimaryKey"></typeparam>
  public interface ILinkTimeline<TTimelineEntry> : ILinkTimeline<string, TTimelineEntry>
      where TTimelineEntry : ITimelineSatellite<string> {
  }

  /// <summary>
  /// default interface for the DataVault v2 Satellite that stores timeline history of a compatible Link entry <br></br>
  /// is used in conjunction with <see cref="ILinkTimeline{TTimelineEntry}"/> satellite to provide all functionality
  /// </summary>
  /// <typeparam name="TLinkReference"></typeparam>
  public interface ITimelineSatellite : ITimelineSatellite<string> {
  }
}
