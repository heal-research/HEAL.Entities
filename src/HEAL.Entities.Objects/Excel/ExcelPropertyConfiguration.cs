using System;

namespace HEAL.Entities.Objects.Excel {

  /// <summary>
  /// stores parsing and mapping information for one property and it's assigned excel column
  /// </summary>
  public class ExcelPropertyConfiguration : ICloneable {
    /// <summary>
    /// 1-based column index
    /// </summary>
    public int ColumnIndex { get; set; }
    /// <summary>
    /// default value for this column if value of cell is null
    /// </summary>
    public object DefaultValue { get; set; }
    /// <summary>
    /// allows for custom parsing structure
    /// </summary>
    public Func<string, object> CellParserFunction { get; set; }
    /// <summary>
    /// aktual column header name as specified in the excel file
    /// </summary>
    public string ColumnHeader { get; set; }
    /// <summary>
    /// name of the target property of the domain objects
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// kind of audit information stored by this property
    /// </summary>
    public ExcelAuditProperties AuditPropertyType { get; set; } = ExcelAuditProperties.NO_AUDIT_FIELD;

    public object Clone() {
      return this.MemberwiseClone();
    }


    /// <summary>
    /// returns the excel column address for an int index, e.g. 'A' for 1, or 'AB' for 28
    /// </summary>
    /// <param name="columnNumber"></param>
    /// <returns></returns>
    public static string ColumnAdress(int columnNumber) {
      int dividend = columnNumber;
      string columnName = String.Empty;
      int modulo;

      while (dividend > 0) {
        modulo = (dividend - 1) % 26;
        columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
        dividend = (int)((dividend - modulo) / 26);
      }

      return columnName;
    }

    /// <summary>
    /// receives an excel column address and resturns the associated index, e.g. 1 for 'A', or 28 for 'AB' 
    /// </summary>
    /// <param name="colAdress"></param>
    /// <returns></returns>
    public static int ColumnNumber(string colAdress) {
      int[] digits = new int[colAdress.Length];
      for (int i = 0; i < colAdress.Length; ++i) {
        digits[i] = Convert.ToInt32(colAdress[i]) - 64;
      }
      int mul = 1; int res = 0;
      for (int pos = digits.Length - 1; pos >= 0; --pos) {
        res += digits[pos] * mul;
        mul *= 26;
      }
      return res;
    }
  }
}
