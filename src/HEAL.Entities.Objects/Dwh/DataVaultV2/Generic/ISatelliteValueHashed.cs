
using System;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {
  /// <summary>
  /// derive from DataVaultSatelliteObject for any Data-Vault LINK entity
  ///  already provides <see cref="BaseDataVaultObject{TPKey}.RecordSource"/> <see cref="BaseDataVaultObject{TPKey}.LoadDate"/> <see cref="DomainObject{T}.PrimaryKey"/>
  ///  a satellite does not save the unique business-key of the HUB entity. and does therefore not implement <see cref="IBusinesKeyed"/>
  /// </summary>
  /// <typeparam name="TSatelliteKey">struct of all attributes defining the primary key</typeparam>
  public interface ISatelliteValueHashed<THubReference, TValueHash> : ISatellite<THubReference>
      where THubReference : IComparable<THubReference>
      where TValueHash : IComparable<TValueHash> {

    /// <summary>
    /// Hash value build over the <see cref="GetValueString"/> representation
    /// </summary>
    TValueHash ValueHash { get; set; }

    /// <summary>
    /// returns a string representation of all datavault relevant values of the satellite
    /// </summary>
    /// <returns>string representation of object values</returns>
    string GetValueString();
  }
}
