using System;

namespace HEAL.Entities.Objects.Excel {
  /// <summary>
  /// List of availtable properties that the ExcelRepository provides
  /// </summary>
  public enum ExcelAuditProperties {
    NO_AUDIT_FIELD,

    /// <summary>
    /// RowId of the entry in its source excel file
    /// ONLY APPLY TO PROPERTIES OF TYPE <see cref="long"/>
    /// </summary>
    RowId
  }
}
