using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HEAL.Entities.DataAccess.Abstractions;
using HEAL.Entities.DataAccess.EPPlus.AttributeConfiguration.Tests;
using HEAL.Entities.DataAccess.EPPlus.ContextConfiguration.Tests;
using HEAL.Entities.DataAccess.Excel;
using HEAL.Entities.DataAccess.Excel.Abstractions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HEAL.Entities.DataAccess.EPPlus.Tests {

  public static class Factory {


    /// <summary>
    /// create default options
    /// </summary>
    /// <returns></returns>
    public static ExcelOptions GetOptions() {
      return new ExcelOptions();
    }

    /// <summary>
    /// create default file options
    /// </summary>
    /// <returns></returns>
    public static ExcelFileOptions GetExcelFileOptions() {
      return new ExcelFileOptions(new FileStream(@"Data\TestData.xlsx", FileMode.Open));
    }

    /// <summary>
    /// Create all the ojects for the attribute based configuration method
    /// </summary>
    public static class Attributed {
      public static ExcelContext GetContext() {
        //create a base context
        var context = new ExcelContext();
        //tell it to map your entity (parses the custom attributes)
        context.BuildAttributedEntity<DomainObject_Attributes>();
        //use the context for your repository
        return context;
      }
      public static IExcelRepository<DomainObject_Attributes, int> GetRepo_LengthDelimited(ExcelContext context,
                          ExcelFileOptions fileOptions,
                          ILogger<EPPlusDomainRepository<DomainObject_Attributes, int>> logger = null) {
        fileOptions.DataMaximumLineNumber = UnitTestMetadata.DelimitedExcelRepo_MaxmimumLineNumber;
        return new EPPlusDomainRepository<DomainObject_Attributes, int>(context,
                                                                      fileOptions,
                                                                      logger);
      }
      public static IExcelRepository<DomainObject_Attributes, int> GetRepo_EndIndicator(ExcelContext context,
                          ExcelFileOptions fileOptions,
                          ILogger<EPPlusDomainRepository<DomainObject_Attributes, int>> logger = null) {
        //how does the Domain object look like when no data is available anymore?
        //in our case primary key will still have the row id but both name fields will be null or empty string
        context.AddEndDelimiter<DomainObject_Attributes>(x => string.IsNullOrEmpty(x.Prename) && string.IsNullOrEmpty(x.Surname));

        return new EPPlusDomainRepository<DomainObject_Attributes, int>(context,
                                                                      fileOptions,
                                                                      logger);
      }
    }

    /// <summary>
    /// create all the objects for the Fluent API based configuration method
    /// </summary>
    public static class FluentApi {
      public static ExcelContext GetContext() {
        //create the new context, calls the OnCreating method that countains the actual mapping
        return new TestExcelContext(GetOptions());
      }

      public static IExcelRepository<DomainObject_FluentApi, int> GetRepo_LengthDelimited(ExcelContext context,
                          ExcelFileOptions fileOptions,
                          ILogger<EPPlusDomainRepository<DomainObject_FluentApi, int>> logger = null) {

        fileOptions.DataMaximumLineNumber = UnitTestMetadata.DelimitedExcelRepo_MaxmimumLineNumber;

        return new EPPlusDomainRepository<DomainObject_FluentApi, int>(context,
                                                                      fileOptions,
                                                                      logger);
      }
      public static IExcelRepository<DomainObject_FluentApi, int> GetRepo_EndIndicator(ExcelContext context,
                          ExcelFileOptions fileOptions,
                          ILogger<EPPlusDomainRepository<DomainObject_FluentApi, int>> logger = null) {

        //how does the Domain object look like when no data is available anymore?
        //in our case primary key will still have the row id but both name fields will be null or empty string
        context.AddEndDelimiter<DomainObject_FluentApi>(x => string.IsNullOrEmpty(x.Prename) && string.IsNullOrEmpty(x.Surname));

        return new EPPlusDomainRepository<DomainObject_FluentApi, int>(context,
                                                                      fileOptions,
                                                                      logger); ;
      }
    }

  }
}
