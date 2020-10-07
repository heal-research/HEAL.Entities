
using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {

  public interface IStoreSatellitePrimaryKey<THubReference> : IStoreLoadDate
                                where THubReference : IComparable<THubReference> {
                                      
    /// <summary>
    /// Reference to the Hub/Link
    /// </summary>
    THubReference Reference { get; set; }

  }

  /// <summary>
  /// Interface for the DataVault v2 satellite primary key <br></br>
  /// Consists of a composite primary key of LoadDate and Hub/Link Reference   <br></br>
  /// <see cref="SatellitePrimaryKey{THubReference}"/> provides a default implementation
  /// </summary>
  /// <typeparam name="THubReference">type of the hash-key</typeparam>
  public interface ISatellitePrimaryKey<THubReference> : IStoreSatellitePrimaryKey<THubReference> , IComparable<ISatellitePrimaryKey<THubReference>>
    where THubReference : IComparable<THubReference> {

  }

  /// <summary>
  /// Default implementation of <see cref="ISatellitePrimaryKey{THubReference}"/>
  /// </summary>
  /// <typeparam name="THubReference"></typeparam>
  public class SatellitePrimaryKey<THubReference> : ISatellitePrimaryKey<THubReference>
                             where THubReference : IComparable<THubReference> {
    public SatellitePrimaryKey() {

    }
    public SatellitePrimaryKey(THubReference reference, DateTime loadDate) {
      this.Reference = reference;
      this.LoadDate = loadDate;
    }

    public THubReference Reference { get; set; }
    public DateTime LoadDate { get; set; }

    public int CompareTo(ISatellitePrimaryKey<THubReference> other) {
      int val = this.Reference.CompareTo(this.Reference);
      if (val != 0) return val;
      return this.LoadDate.CompareTo(other.LoadDate);
    }
  }

  /// <summary>
  /// Interface for the DataVault v2 type Satellite
  /// </summary>
  /// <typeparam name="THubReference"></typeparam>
  public interface ISatellite<THubReference> : IDataVaultObject<ISatellitePrimaryKey<THubReference>>, IStoreSatellitePrimaryKey<THubReference>
      where THubReference : IComparable<THubReference> {
  }
}
