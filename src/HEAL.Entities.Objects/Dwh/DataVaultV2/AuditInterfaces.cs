using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {

  /// <summary>
  /// classes that extend <see cref="BaseDataVaultObject{TPKey, TBKey}"/> should implement this interface if they reference load information
  /// </summary>
  public interface IReferenceLoadInformation : IReferenceLoadInformation<long> {
  }

  /// <summary>
  /// DataVault Objects need to implement any interface derived from <see cref="IAuditInformation"/> to fully support the DataVault idea
  /// This load data (Timestamp, record source, loading user/application) can be either stored by the entity itself <see cref="IReferenceLoadInformation"/>
  /// or stored in a seperate table where the entry is only referenced, implemented by <see cref="IReferenceLoadInformation{TPKey}"/>
  /// </summary>
  public interface IAuditInformation {
  }

  /// <summary>
  /// classes that extend <see cref="BaseDataVaultObject{TPKey, TBKey}"/> should implement this interface if they want to directly store load information
  /// </summary>
  public interface IStoreLoadInformation : IAuditInformation {
    /// <summary>
    /// traceable description or reference to the source of information
    /// </summary>
    string RecordSource { get; set; }
    /// <summary>
    /// traceable information about the load programm and infrastructure. i.e. who, what and in which version imported this information
    /// </summary>
    string LoadedBy { get; set; }
  }

  /// <summary>
  /// optional additional autid attribute
  /// classes extending <see cref="BaseDataVaultObject{TPKey, TBKey}"/> should implement this interface if they require auditing of editor information
  /// </summary>
  public interface IEditedByInformation {
    string EditedBy { get; set; }
  }
}
