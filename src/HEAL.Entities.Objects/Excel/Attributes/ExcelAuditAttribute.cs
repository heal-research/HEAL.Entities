using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Excel {


  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class ExcelAuditAttribute : Attribute {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="auditProperty"></param>
    public ExcelAuditAttribute(ExcelAuditProperties auditProperty) {
      AuditProperty = auditProperty;
    }

    public ExcelAuditProperties AuditProperty { get; private set; }
  }
}
