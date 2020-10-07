using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {
  /// <summary>
  /// An Implementation of the DataVaultHashFunction-Delegate has to be provided to calculate the PrimaryKey Hash
  ///  as defined in DataVault 2.0
  /// </summary>
  /// <typeparam name="TPKey">Type of the Primary Key</typeparam>
  /// <param name="data">Concatenated string of business key attributes 'Key1Key2KeyN...' without any sepparators</param>
  /// <returns>Calculated hash of the parameters in the defined <see cref="{TPKey}"/> type</returns>
  public delegate TPKey DataVaultHashFunction<TPKey>(string data);


  /// <summary>
  /// Extracted into seperate interface to remove interface implementation ambiguity (Satellite LoadDate vs. IDataVaultLoadDate)
  /// </summary>
  public interface IStoreLoadDate {
    /// <summary>
    /// Stores the exact moment when this entry was first seen by the DataVault Store
    /// </summary>
    DateTime LoadDate { get; set; }
  }

  /// <summary>
  /// Extracted into seperate interface to remove interface implementation ambiguity (Satellite LoadDate vs. IDataVaultLoadDate)
  /// </summary>
  public interface IStoreEndDate {
    /// <summary>
    /// Stores the exact moment when the DataVault Store recognized an entry was deleted and is therefore marked as ended 
    /// </summary>
    DateTime? EndDate { get; set; }
  }

  /// <summary>
  /// Base DataVault Interface describing the features that are share over all DataVault 2.0 objects
  /// </summary>
  /// <typeparam name="TPKey">type of the primary key (e.g. 32 char string of Hash value)</typeparam>
  public interface IDataVaultObject<TPKey> : IDomainObject<TPKey>, IStoreLoadDate, IAuditInformation
                              where TPKey : IComparable<TPKey> {

    /// <summary>
    /// returns a string representation of all businesskey values stored in the object
    /// </summary>
    /// <returns>string representation of object values</returns>
    string GetBusinessKeyString();

  }
}
