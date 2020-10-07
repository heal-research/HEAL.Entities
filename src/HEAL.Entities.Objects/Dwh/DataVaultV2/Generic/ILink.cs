using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {

  /// <summary>
  /// Interface for the default DataVault v2 type Link <br></br>
  /// <see cref="ILinkTimeline{TPKey}"/> if this link can be logically removed
  /// </summary>
  /// <typeparam name="TPKey"></typeparam>
  public interface ILink<TPKey> : IDataVaultObject<TPKey>
      where TPKey : IComparable<TPKey> {
  }
}
