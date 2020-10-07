using System;

namespace HEAL.Entities.DataAccess.Excel {
  public class ExcelOptions : ICloneable, IDisposable {

    /// <summary>
    /// set to true to rethrow exceptions that occur on cell parsing.
    /// If set to false, the default value of the type is applied instead of no value.
    /// </summary>
    public bool PropagageParsingError { get; set; } = false;


    /// <summary>
    /// set to true to rethrow exceptions that occur on cell parsing.
    /// If set to false, the default value of the type is applied instead of no value.
    /// </summary>
    public bool ThrowExceptionOnMismatchingColumnNames { get; set; } = false;


    public object Clone() {
      return this.MemberwiseClone();
    }

    

    public void Dispose() {
    }
  }
}
