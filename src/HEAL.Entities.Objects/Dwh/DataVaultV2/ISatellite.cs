
using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {

  /// <summary>
  /// default implementation of Data Vault V2 and <see cref="ISatellitePrimaryKey{THubReference}"/> for primary key hashes stored as string
  /// </summary>
  public interface ISatellitePrimaryKey : ISatellitePrimaryKey<string> { }
  /// <summary>
  /// default implementation of Data Vault V2 and <see cref="ISatellite{THubReference}"/> for primary key hashes stored as string
  /// </summary>
  public interface ISatellite : ISatellite<string> { }
  /// <summary>
  /// default implementation of Data Vault V2 and <see cref="ISatelliteValueHashed{THubReference, TValueHash}"/> for primary key and value hashes stored as string
  /// </summary>
  public interface IValueHashedSatellite : ISatelliteValueHashed<string, string> { }

}
