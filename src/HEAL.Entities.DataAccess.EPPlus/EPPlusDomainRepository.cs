using HEAL.Entities.DataAccess.Excel;
using HEAL.Entities.DataAccess.Excel.Abstractions;
using HEAL.Entities.Objects;
using HEAL.Entities.Objects.Excel;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HEAL.Entities.DataAccess.Abstractions {

  public class EPPlusDomainRepository<TEntity, TKey> : IExcelRepository<TEntity, TKey>, IDisposable
      where TEntity : class, IDomainObject<TKey>, new()
      where TKey : IComparable<TKey> {

    private PropertyInfo[] EntityProperties = typeof(TEntity).GetProperties();

    /// <summary>
    /// EPPlus <see cref="ExcelPackage"/> used to access the excel file
    /// </summary>
    protected ExcelPackage ExcelPackage { get; set; }

    /// <summary>
    /// EPPlus <see cref="ExcelWorkSheet"/> used to access the worksheet of the excel file
    /// </summary>
    protected ExcelWorksheet ExcelWorkSheet { get; set; }

    /// <summary>
    /// Logger instance for detailed information
    /// </summary>
    protected ILogger<EPPlusDomainRepository<TEntity, TKey>> _logger;

    protected ExcelContext Context { get; set; }

    /// <summary>
    /// returns a clone of the internal excel options. change of parameters is not possible.
    /// </summary>
    public ExcelFileOptions GetExcelFileOptions {
      get { return (ExcelFileOptions)ExcelFileOptions.Clone(); }
    }
    internal ExcelFileOptions ExcelFileOptions;

    internal IDictionary<string, ExcelPropertyBuilder> TargetProperties { get; }

    /// <summary>
    /// create new instance of <see cref="EPPlusDomainRepository"/> <br/>
    /// for more options cosider calling <see cref="EPPlusDomainRepository{TEntity, TKey}.EPPlusDomainRepository(ExcelContext, ExcelFileOptions, ILogger{EPPlusDomainRepository{TEntity, TKey}})"/> and configer <see cref="ExcelFileOptions"/></param>
    /// </summary>
    /// <param name="context">inforamtion about the data context i.e. entity mapping, general file options</param>
    /// <param name="excelFileStream">file location for parsing, for more options cosider calling <see cref="EPPlusDomainRepository{TEntity, TKey}.EPPlusDomainRepository(ExcelContext, ExcelFileOptions, ILogger{EPPlusDomainRepository{TEntity, TKey}})"/> and configer <see cref="ExcelFileOptions"/></param>
    /// <param name="password">password for file access</param>
    /// <param name="logger">used for internal logging prints</param>
    public EPPlusDomainRepository(ExcelContext context, Stream excelFileStream, string password = default,
                                  ILogger<EPPlusDomainRepository<TEntity, TKey>> logger = null)
        : this(context, new ExcelFileOptions(excelFileStream, password), logger) {
    }

    /// <summary>
    /// create new instance of <see cref="EPPlusDomainRepository"/> <br/>
    /// </summary>
    /// <param name="context">inforamtion about the data context i.e. entity mapping, general file options</param>
    /// <param name="excelFileOptions">data and options related to file specific information i.e. file location, data start, data length</param>
    /// <param name="logger">used for internal logging prints</param>
    public EPPlusDomainRepository(ExcelContext context, ExcelFileOptions excelFileOptions,
                                  ILogger<EPPlusDomainRepository<TEntity, TKey>> logger = null) {
      _logger = logger;

      Context = context;
      ExcelFileOptions = excelFileOptions;

      TargetProperties = Context.GetPropertyBuilders<TEntity>();

      ValidateExcelSourceConfiguration(excelFileOptions, context);
      SetExcelDataSource(excelFileOptions);
      ValidateHeaderNames();
    }

    /// <summary>
    /// configures the repository to handle a new kind of data-format (row placement of data)
    /// </summary>
    /// <param name="headerRowIndex">index (1-Based) of the header row</param>
    /// <param name="dataStartRowIndex">index (1-Based) of the first data row</param>
    /// <param name="dataEndRowIndex">index (1-Based) of last valid Row; use 0 = unlimited but EndRowIndicator must be supplied.</param>
    /// <param name="endRowIndicator">returns true if parsed entry is not a valid row anymore. this marked row will be discarded</param>
    protected virtual void ValidateExcelSourceConfiguration(ExcelFileOptions options, ExcelContext context) {

      var excelEntityTypeBuilder = (context.ObtainEntityFromDictionary<TEntity>() as ExcelEntityBuilder<TEntity>);
      if (excelEntityTypeBuilder == null) {
        throw new ArgumentOutOfRangeException(nameof(options.HeaderLineNumber), $"Excel-{nameof(options.HeaderLineNumber)} indices are 1-Based.");
      }

      #region ParameterChecks
      if (options.HeaderLineNumber < 1)
        throw new ArgumentOutOfRangeException(nameof(options.HeaderLineNumber), $"Excel-{nameof(options.HeaderLineNumber)} indices are 1-Based.");
      if (options.DataStartLineNumber < 1)
        throw new ArgumentOutOfRangeException(nameof(options.DataStartLineNumber), $"Excel-{nameof(options.DataStartLineNumber)} indices are 1-Based.");

      if (options.DataMaximumLineNumber != ExcelFileOptions.DEFAULT_END_UNLIMITED && options.DataStartLineNumber > options.DataMaximumLineNumber)
        throw new ArgumentOutOfRangeException($"{nameof(options.DataStartLineNumber)}{nameof(options.DataMaximumLineNumber)}"
          , $"{nameof(options.DataStartLineNumber)} must not be higher than {nameof(options.DataMaximumLineNumber)}");
      #endregion
    }

    /// <summary>
    /// configures the repository for a new file-source
    /// </summary>
    /// <param name="file">Path to the source excel file</param>
    /// <param name="workSheetName">name of the worksheet in the file</param>
    protected virtual void SetExcelDataSource(ExcelFileOptions excelFileOptions) {
      ExcelPackage = new ExcelPackage(excelFileOptions.ExcelFileStream, excelFileOptions.FilePassword);
      ExcelWorkSheet = ExcelPackage.Workbook.Worksheets[excelFileOptions.WorksheetName];
    }

    internal void ValidateHeaderNames() {

      foreach (var targetPropPair in TargetProperties) {
        var propertyName = targetPropPair.Key;
        if (!EntityProperties.Select(x => x.Name).Contains(propertyName))
          throw new ArgumentOutOfRangeException($"Property of name '{propertyName}' was configured for type '{typeof(TEntity)}'" +
            $" but is not contained in the reflection properties.");
        var targetPropertyConfiguration = targetPropPair.Value;

        if (targetPropertyConfiguration.Configuration.AuditPropertyType != ExcelAuditProperties.NO_AUDIT_FIELD)
          continue;

        //get configured header name
        var configuredHeaderName = targetPropertyConfiguration.Configuration.ColumnHeader;
        string excelHeaderCellValue = ExtractHeaderCellValue(targetPropertyConfiguration.Configuration.ColumnIndex);

        //check header and property name
        if (!(configuredHeaderName == excelHeaderCellValue
                 || propertyName.Equals(excelHeaderCellValue))) {
          var errorMessage = $"Neither Configured excel {nameof(ExcelPropertyConfiguration.ColumnHeader)} value '{configuredHeaderName}'" +
            $" NOR the property name '{propertyName}' do match the header value '{excelHeaderCellValue}'."
                  + $" This might indicate a fault in your configuration or a new version of excel data that does not match the app configuration.";
          _logger?.LogWarning(errorMessage);
          if (Context.GetExcelOptions.ThrowExceptionOnMismatchingColumnNames)
            throw new ArgumentException(errorMessage);
        }
      }
    }

    protected virtual string ExtractHeaderCellValue(int columnIndex) {
      return ExcelWorkSheet.Cells[ExcelFileOptions.HeaderLineNumber, columnIndex].GetValue<string>();
    }

    public virtual IEnumerable<TEntity> GetAll() {
      var lastLineDetector = Context.GetExcelEntityTypeBuilder<TEntity>().LastRowDetector;

      //iterate through all entries from start to finish row 
      // or parse indefinetely and wait for break through end-row indicator
      for (int rowIndex = ExcelFileOptions.DataStartLineNumber;
        ExcelFileOptions.DataMaximumLineNumber == ExcelFileOptions.DEFAULT_END_UNLIMITED || rowIndex <= ExcelFileOptions.DataMaximumLineNumber;
        rowIndex++) {

        TEntity row = ParseDataRow(rowIndex);

        //if parsed entry indicates end of data (user defined predicate) parsing is finished/stoped
        // end row must not be returned
        if (lastLineDetector != null && lastLineDetector(row))
          break;

        yield return row;
      }
    }

    protected virtual TEntity ParseDataRow(int rowIndex) {
      TEntity row = new TEntity();

      var parsedEntityType = typeof(TEntity);

      //iterate through all properties of the entity instead of the excel columns
      foreach (var targetPropPair in TargetProperties) {
        var propertyName = targetPropPair.Key;
        var columnConfiguration = targetPropPair.Value.Configuration;

        var targetProperty = EntityProperties.Single(x => x.Name == propertyName);
        if (targetProperty == null)
          throw new ArgumentOutOfRangeException($"Property of name '{propertyName}' was configured for type '{typeof(TEntity)}' " +
            $"but is not contained in the reflection properties.");

        //only continue when current property is data property and not an audit field
        if (columnConfiguration.AuditPropertyType == ExcelAuditProperties.NO_AUDIT_FIELD) {
          ParseConfiguredColumn(rowIndex, row, targetProperty, columnConfiguration);
        } else {
          HandleAuditProperty(rowIndex, row, targetProperty, columnConfiguration);
        }
      }

      return row;
    }

    protected virtual void HandleAuditProperty(int rowIndex, TEntity row, PropertyInfo targetProperty, ExcelPropertyConfiguration columnConfiguration) {
      switch (columnConfiguration.AuditPropertyType) {
        case ExcelAuditProperties.RowId:
          if (targetProperty.PropertyType == typeof(int))
            targetProperty.SetValue(row, rowIndex);
          return;
        default:
          throw new NotImplementedException($"Error in '{nameof(EPPlusDomainRepository<TEntity, TKey>)}'. " +
            $"The method'{nameof(HandleAuditProperty)}' does not support the enum '{nameof(ExcelAuditProperties)}' value of '{columnConfiguration.AuditPropertyType}'.");
      }
    }

    protected virtual void ParseConfiguredColumn(int rowIndex, TEntity row, PropertyInfo targetProperty, ExcelPropertyConfiguration columnConfiguration) {
      

      //Excel attribute defined the target column
      var propertyColIndex = columnConfiguration.ColumnIndex;

      if (ExcelWorkSheet == null)
        throw new NullReferenceException($"Error in '{nameof(EPPlusDomainRepository<TEntity, TKey>)}' the property '{nameof(ExcelWorkSheet)}'" +
          $" is null. The repository was most likely disposed before an IEnumberable was evaluated.");

      if (ExcelWorkSheet.Cells[rowIndex, propertyColIndex].Value == null) {
        targetProperty.SetValue(row, columnConfiguration.DefaultValue);
      } else {
        if (columnConfiguration.CellParserFunction != null) {
          //custom string parser was supplied
          var stringValue = ExcelWorkSheet.Cells[rowIndex, propertyColIndex].Text;
          var value = columnConfiguration.CellParserFunction(stringValue);
          targetProperty.SetValue(row, value ?? columnConfiguration.DefaultValue);
        } else {
          try {
            ParseGenericCellValue(row, rowIndex, propertyColIndex, targetProperty);
          }
          catch (Exception e) {
            if (Context.Options.PropagageParsingError)
              throw e;

            _logger?.LogError(e, "Generic Parsing of Value was not possible. Default value will be applied");
            targetProperty.SetValue(row, columnConfiguration.DefaultValue);
          }
        }
      }
    }

    protected virtual void ParseGenericCellValue(TEntity row, int rowIndex, int colIndex, System.Reflection.PropertyInfo targetProp) {
      //use generic parser of ExcelRange
      var getValueMethod = typeof(ExcelRange).GetMethod(nameof(ExcelRange.GetValue));
      var genericGetValueMethod = getValueMethod.MakeGenericMethod(targetProp.PropertyType);
      var value = genericGetValueMethod.Invoke(ExcelWorkSheet.Cells[rowIndex, colIndex], null);
      targetProp.SetValue(row, value);
    }

    protected virtual object GetDefaultValue(Type t) {
      if (t.IsValueType)
        return Activator.CreateInstance(t);

      return null;
    }

    protected virtual IEnumerable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filter = null) {
      Func<TEntity, bool> func = e => true;
      if (filter != null)
        func = filter.Compile();

      foreach (var entry in GetAll()) {
        if (func(entry))
          yield return entry;
      }
    }

    public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "") {
      foreach (var entry in orderBy.Invoke(GetFiltered(filter).AsQueryable())) {
        yield return entry;
      }
    }

    public virtual long Count(Expression<Func<TEntity, bool>> filter = null) {
      return GetFiltered(filter).AsQueryable().LongCount();
    }

    /// <summary>
    /// implemented with yield return
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual TEntity GetByRowId(int id) {
      return ParseDataRow(id);
    }

    public virtual TEntity GetByKey(TKey id) {
      foreach (var entry in GetAll()) {
        if (entry.PrimaryKey.Equals(id))
          //stops the yield return of GetAll()
          return entry;
      }
      //no element found
      return default(TEntity);
    }

    public virtual void Dispose() {
      //Dispose EPPlus components
      ExcelWorkSheet?.Dispose();
      ExcelPackage?.Dispose();
    }

  }
}
