namespace HEAL.Entities.Utils.Data {
  public class DataDictionaryComparisonConfiguration {

    /// <summary>
    /// determines how many decimal places are relevant for comparison <br></br>
    /// null is default and ensures application of default CompareTo 
    /// </summary>
    public int? DecimalPrecision { get; set; } = null;

    /// <summary>
    /// determines how many decimal places are relevant for comparison <br></br>
    /// null is default and ensures application of default CompareTo 
    /// </summary>
    public int? FloatingPointPrecision { get; set; } = null;

  }
}