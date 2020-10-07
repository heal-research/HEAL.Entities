using System;
using System.Linq;
using HEAL.Entities.Objects.Excel;

namespace HEAL.Entities.DataAccess.EPPlus.Tests {
  public class MaritalStatusParser : ICellParser {
    public object ParseValue(string cellValue) {
      switch (cellValue) {
        case "Married": return true;
        case "Single": return false;
        default:
          throw new NotSupportedException();
      }
    }
  }

  public class CommaSeparatedListParser : ICellParser {
    public object ParseValue(string cellValue) {
      if (string.IsNullOrEmpty(cellValue))
        return new string[0];
      return cellValue.Split(',').Select(x => x.Trim()).ToArray();
    }
  }
}
