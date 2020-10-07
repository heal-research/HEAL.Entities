using System;
using System.Collections;
using System.Text;

namespace HEAL.Entities.Objects.Excel {

  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class ExcelColumnConfigurationAttribute : Attribute {
    /// <summary>
    /// Configures how this specific attribute is parsed from an Excel-File.
    /// </summary>
    /// <param name="ColumnIndex">1-Bases index of the column in the Excel-File</param>
    /// <param name="DefaultValue">Default-Value used if cell values is null or empty; Excel-Value is not replaced if <see cref="DefaultValue"/> is kept null</param>
    /// <param name="CellParser">Parses a string value to the property-type. This Type implements <see cref="ICellParser"/> with empty constructor.</param>
    /// <param name="WriteStyleNumberformatFormat">e.g. '€#,##0.00': used for writing back into excel to specify cell format </param>
    public ExcelColumnConfigurationAttribute(ExcelColumnEnum ColumnIndex, string ColumnHeaderText = null, object DefaultValue = null, Type cellParser = null)
     : this((int)ColumnIndex, ColumnHeaderText, DefaultValue, cellParser) {
    }

    /// <summary>
    /// Configures how this specific attribute is parsed from an Excel-File.
    /// </summary>
    /// <param name="ColumnIndex">e.g. 'A' or 'AB'</param>
    /// <param name="DefaultValue">Default-Value used if cell values is null or empty; Excel-Value is not replaced if <see cref="DefaultValue"/> is kept null</param>
    /// <param name="CellParser">Parses a string value to the property-type. This Type implements <see cref="ICellParser"/> with empty constructor.</param>
    /// <param name="WriteStyleNumberformatFormat">e.g. '€#,##0.00': used for writing back into excel to specify cell format </param>
    public ExcelColumnConfigurationAttribute(string columnAddress, string ColumnHeaderText = null, object DefaultValue = null, Type cellParser = null)
     : this(ExcelPropertyConfiguration.ColumnNumber(columnAddress), ColumnHeaderText, DefaultValue, cellParser) {
    }


    /// <summary>
    /// Configures how this specific attribute is parsed from an Excel-File.
    /// </summary>
    /// <param name="ColumnIndex">1-Bases index of the column in the Excel-File</param>
    /// <param name="DefaultValue">Default-Value used if cell values is null or empty; Excel-Value is not replaced if <see cref="DefaultValue"/> is kept null</param>
    /// <param name="cellParser">Parses a string value to the property-type.</param>
    /// <param name="WriteStyleNumberFormat">e.g. '€#,##0.00': used for writing back into excel to specify cell format </param>
    public ExcelColumnConfigurationAttribute(int ColumnIndex, string ColumnHeaderText = null, object DefaultValue = null, Type cellParser = null) {
      if (ColumnIndex < 1)
        throw new ArgumentOutOfRangeException(nameof(ColumnIndex), $"Excel-{nameof(ColumnIndex)} starts with 1.");
      configuration = new ExcelPropertyConfiguration();
         

      this.configuration.ColumnIndex = ColumnIndex;
      this.configuration.DefaultValue = DefaultValue;
      this.configuration.ColumnHeader = ColumnHeaderText;

      //if cell parser is supplied is has to implement the correct interface
      if (cellParser != null) {
        if (cellParser.GetInterface(nameof(ICellParser)) == null)
          throw new ArgumentOutOfRangeException(nameof(cellParser), $"{nameof(cellParser)} must implement HEAL.Entities.Objects.Excel.ICellParser.");
        //create instance
        var instance = (ICellParser)Activator.CreateInstance(cellParser);

        this.configuration.CellParserFunction = instance.ParseValue;
      } else {
        this.configuration.CellParserFunction = null;
      }
    }

    private ExcelPropertyConfiguration configuration;

    public ExcelPropertyConfiguration Configuration => (ExcelPropertyConfiguration) configuration.Clone();
  }
}
