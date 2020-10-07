using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Dwh.DataVaultV2 {



  /// <summary>
  /// classes that extend <see cref="BaseDataVaultObject{THubReference, TBKey}"/> should implement this interface if they reference load information
  /// </summary>
  public interface IReferenceLoadInformation<THubReference> : IAuditInformation {
    THubReference LoadReference { get; set; }
  }


}
