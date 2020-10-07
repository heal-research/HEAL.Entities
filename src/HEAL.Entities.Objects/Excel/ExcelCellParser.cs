using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Excel {

  /// <summary>
  /// needed as c# attributes do not allow functions or delegates as types for constructor parameters. <br></br>
  /// to circumvent this restriction the type of any class implementing <see cref="ICellParser"/> can be passed and its default constructur will be evoked.
  /// </summary>
  public interface ICellParser {
    object ParseValue(string cellValue);
  }

  public class BinaryBooleanParser : ICellParser {
    public object ParseValue(string cellValue) {
      return cellValue.ParseBinaryNullableBoolean();
    }
  }

  /// <summary>
  /// contains various static methods for boolean string parsing. e.g. binary 0/1 or german ja/nein
  /// </summary>
  public static class DefaultBooleanParser {
    public static bool? ParseNullableBoolean(this string cellValue) {
      bool value;
      return Boolean.TryParse(cellValue, out value) ? value : default(bool?);
    }

    const string binaryNo = "0";
    const string binaryYes = "1";
    /// <summary>
    /// parsing binary 0/1 as boolean true/false
    /// </summary>
    /// <param name="cellValue"></param>
    /// <returns></returns>
    public static bool? ParseBinaryNullableBoolean(this string cellValue) {
      switch (cellValue) {
        case binaryNo:
          return false;
        case binaryYes:
          return true;
        default:
          return null;
      }
    }
  }
}
