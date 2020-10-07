using System;
using HEAL.Entities.Objects.Excel;

namespace HEAL.Entities.DataAccess.Excel {

  /// <summary>
  /// non-generic base class of <see cref="ExcelPropertyBuilder{TProperty}"/> that contains all 
  /// properties and methods that are not specific to the property tyep
  /// </summary>
  public class ExcelPropertyBuilder {
    /// <summary>
    /// column configuration that contains the actual mapping of one individual property to the excel column
    /// this class is the most important part of the mapping process as it is used by the repository implementation
    /// </summary>
    internal ExcelPropertyConfiguration Configuration { get; set; } = new ExcelPropertyConfiguration();

  }

  /// <summary>
  /// contains all fluent api methods for the configuration of one individual property of the domain object
  /// </summary>
  /// <typeparam name="TProperty"></typeparam>
  public class ExcelPropertyBuilder<TProperty> : ExcelPropertyBuilder {

    /// <summary>
    /// specify the excel column adress of the property
    /// </summary>
    /// <param name="excelColumn"></param>
    /// <returns></returns>
    public ExcelPropertyBuilder<TProperty> Column(ExcelColumnEnum excelColumn) {
      return Column((int)excelColumn);
    }

    /// <summary>
    /// specify the excel column adress of the property
    /// </summary>
    /// <param name="excelColumn"></param>
    /// <returns></returns>
    public ExcelPropertyBuilder<TProperty> Column(string columnName) {
      return Column(ExcelPropertyConfiguration.ColumnNumber(columnName));
    }

    public ExcelPropertyBuilder<TProperty> Column(int columnIndex) {
      if(Configuration.ColumnIndex != default)
        throw new ArgumentException($"The configuration method '{nameof(Column)}' was already called once for the property '{Configuration.PropertyName}'.");
      this.Configuration.ColumnIndex = columnIndex;
      return this;
    }

    /// <summary>
    /// specify the excel column adress of the property
    /// </summary>
    /// <param name="excelColumn"></param>
    /// <returns></returns>
    public ExcelPropertyBuilder<TProperty> WithDefault(TProperty defaultValue) {
      if (Configuration.DefaultValue != default)
        throw new ArgumentException($"The configuration method '{nameof(WithDefault)}' was already called once for the property '{Configuration.PropertyName}'.");
      this.Configuration.DefaultValue = defaultValue;
      return this;
    }

    /// <summary>
    /// specify the parser function to be used for this property
    /// </summary>
    /// <param name="CellParserFunction"></param>
    /// <returns></returns>
    public ExcelPropertyBuilder<TProperty> UseCustomParser(Func<string, TProperty> CellParserFunction) {
      if (Configuration.CellParserFunction != default)
        throw new ArgumentException($"The configuration method '{nameof(UseCustomParser)}' was already called once for the property '{Configuration.PropertyName}'.");
      this.Configuration.CellParserFunction = (s) => (object)CellParserFunction(s);
      return this;
    }

    /// <summary>
    /// specify the parser implementation to be used for this property
    /// </summary>
    /// <param name="CellParserFunction"></param>
    /// <returns></returns>
    public ExcelPropertyBuilder<TProperty> UseCustomParser(ICellParser cellParser) {
      if (Configuration.CellParserFunction != default)
        throw new ArgumentException($"The configuration method '{nameof(UseCustomParser)}' was already called once for the property '{Configuration.PropertyName}'.");
      this.Configuration.CellParserFunction = (s) => (object)cellParser.ParseValue(s);
      return this;
    }

    /// <summary>
    /// specify the header name of this column
    /// </summary>
    /// <param name="columnHeaderName"></param>
    /// <returns></returns>
    public ExcelPropertyBuilder<TProperty> HasHeaderName(string columnHeaderName) {
      if (Configuration.ColumnHeader != default)
        throw new ArgumentException($"The configuration method '{nameof(HasHeaderName)}' was already called once for the property '{Configuration.PropertyName}'.");
      this.Configuration.ColumnHeader = columnHeaderName;
      return this;
    }
  }
}
