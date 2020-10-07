using System;
using System.Collections.Generic;
using System.Text;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {
  public static class Extensions {


    /// <summary>
    /// creates a <see cref="SatellitePrimaryKey{THubReference}"/> object for the reference and loaddate
    /// use this extension methog e.g. in the getter of  <see cref="IDomainObject{T}.PrimaryKey"/> in a class that implements <see cref="ISatellite"/> "/>
    /// </summary>
    /// <param name="satellite"></param>
    /// <typeparam name="THubReference">type used for hash references according to DataVaultV2</typeparam>
    /// <returns></returns>
    public static ISatellitePrimaryKey<THubReference> GetSatellitePrimaryKey<TSatellite, THubReference>(this TSatellite satellite)
          where TSatellite : IDataVaultObject<ISatellitePrimaryKey<THubReference>>, IStoreSatellitePrimaryKey<THubReference>
        where THubReference : IComparable<THubReference> {
      return new SatellitePrimaryKey<THubReference>(satellite.Reference, satellite.LoadDate);
    }

    /// <summary>
    /// creates a <see cref="SatellitePrimaryKey{THubReference}"/> object for the reference and loaddate
    /// use this extension methog e.g. in the getter of  <see cref="IDomainObject{T}.PrimaryKey"/> in a class that implements <see cref="ISatellite"/> "/>
    /// </summary>
    /// <param name="satellite"></param>
    /// <returns></returns>
    public static ISatellitePrimaryKey<string> GetSatellitePrimaryKey<TSatellite>(this TSatellite satellite)
          where TSatellite : IDataVaultObject<ISatellitePrimaryKey<string>>, IStoreSatellitePrimaryKey<string> {
      return GetSatellitePrimaryKey<TSatellite, string>(satellite);
    }


    /// <summary>
    /// sets the <see cref="ISatellitePrimaryKey.Reference"/> and <see cref="IStoreLoadDate.LoadDate"/> property of the given <see cref="ISatellite"/>
    /// use this exteions e.g. in a setter of <see cref="IDomainObject{T}.PrimaryKey"/> in a class that implements <see cref="ISatellite"/>
    /// </summary>                         
    /// <param name="satellite">satellite where primary key should be set</param>
    /// <param name="primaryKey">the primay key values</param>
    /// /// <typeparam name="THubReference">type used for hash references according to DataVaultV2</typeparam>
    public static void SetSatellitePrimaryKey<TSatellite, THubReference>(this TSatellite satellite, ISatellitePrimaryKey<THubReference> primaryKey)
        where TSatellite : IDataVaultObject<ISatellitePrimaryKey<THubReference>>, IStoreSatellitePrimaryKey<THubReference>
        where THubReference : IComparable<THubReference> {
      satellite.Reference = primaryKey.Reference;
      satellite.LoadDate = primaryKey.LoadDate;
    }

    /// <summary>
    /// sets the <see cref="ISatellitePrimaryKey.Reference"/> and <see cref="IStoreLoadDate.LoadDate"/> property of the given <see cref="ISatellite"/>
    /// use this exteions e.g. in a setter of <see cref="IDomainObject{T}.PrimaryKey"/> in a class that implements <see cref="ISatellite"/>
    /// </summary>                         
    /// <param name="satellite">satellite where primary key should be set</param>
    /// <param name="primaryKey">the primay key values</param>
    public static void SetSatellitePrimaryKey<TSatellite>(this TSatellite satellite, ISatellitePrimaryKey<string> primaryKey)
        where TSatellite : IDataVaultObject<ISatellitePrimaryKey<string>>, IStoreSatellitePrimaryKey<string> {
      SetSatellitePrimaryKey<TSatellite, string>(satellite, primaryKey);
    }

  }
}
