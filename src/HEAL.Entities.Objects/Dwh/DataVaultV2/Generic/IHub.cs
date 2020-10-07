using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {
  /// <summary>
  /// Interface for the DataVault v2 type hub
  /// </summary>
  /// <typeparam name="TPKey">type of the hash-key</typeparam>
  public interface IHub<TPKey> : IDataVaultObject<TPKey>
      where TPKey : IComparable<TPKey> { }
}
