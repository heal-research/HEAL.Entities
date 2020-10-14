using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Entities.DataAccess.Abstractions;
using HEAL.Entities.DataAccess.EPPlus.AttributeConfiguration.Tests;
using HEAL.Entities.DataAccess.EPPlus.ContextConfiguration.Tests;
using HEAL.Entities.DataAccess.EPPlus.Tests;
using Xunit;

namespace HEAL.Entities.DataAccess.EPPlus.Tests {


  [Collection("Sequential")]
  public class RepositoryTests {
    [Fact]
    public void NoNegativeIndicesInConstructor() {
      Assert.Throws<ArgumentOutOfRangeException>(() => {
        using (var options = Factory.GetExcelFileOptions()) {
          options.HeaderLineNumber = -1;  //negative header row
          using (var repo = new EPPlusDomainRepository<DomainObject_FluentApi, int>(Factory.FluentApi.GetContext(), options)) { }
        }
      });
      Assert.Throws<ArgumentOutOfRangeException>(() => {
        using (var options = Factory.GetExcelFileOptions()) {
          options.DataStartLineNumber = -1;   //negative header row
          using (var repo = new EPPlusDomainRepository<DomainObject_FluentApi, int>(Factory.FluentApi.GetContext(), options)) { }
        }
      });
      Assert.Throws<ArgumentOutOfRangeException>(() => {
        using (var options = Factory.GetExcelFileOptions()) {
          options.DataMaximumLineNumber = -1; //negative header row
          using (var repo = new EPPlusDomainRepository<DomainObject_FluentApi, int>(Factory.FluentApi.GetContext(), options)) { }
        }
      });
    }

    [Fact]
    public void EndCanBeDelimitted() {
      IEnumerable<ITestDomainObject> testData_Attributes;
      using (var context = Factory.FluentApi.GetContext())
      using (var fileOptions = Factory.GetExcelFileOptions())
      using (var repo = Factory.FluentApi.GetRepo_EndIndicator(context, fileOptions)) {
        Assert.Equal(UnitTestMetadata.ExcelDataCount, repo.Count());
        Assert.Equal(UnitTestMetadata.ExcelDataCount, repo.GetAll().Count());
      }
    }

    [Fact]
    public void TwoConfigurationVariants() {
      IEnumerable<ITestDomainObject> testData_Attributes;
      using (var context = Factory.Attributed.GetContext())
      using (var fileOptions = Factory.GetExcelFileOptions())
      using (var repo = new EPPlusDomainRepository<DomainObject_Attributes, int>(context, fileOptions)) {
        //read all data from the (same) excel using the attribute based access method
        testData_Attributes = repo.GetAll().Take(UnitTestMetadata.ExcelDataCount).ToList();
      }

      IEnumerable<ITestDomainObject> testData_FluentApi;
      using (var context = Factory.FluentApi.GetContext())
      using (var fileOptions = Factory.GetExcelFileOptions())
      using (var repo = new EPPlusDomainRepository<DomainObject_FluentApi, int>(context, fileOptions)) {
        //read all data from the (same) excel using the Fluent API based access method
        testData_FluentApi = repo.GetAll().Take(UnitTestMetadata.ExcelDataCount).ToList();
      }

      for (int i = 0; i < UnitTestMetadata.ExcelDataCount; i++) {
        //data should be the same
        UnitTestMetadata.AssertEqual(testData_Attributes.ElementAt(i), testData_FluentApi.ElementAt(i));
      }
    }

    [Fact]
    public void GetByKeyOrRowId() {
      using (var context = Factory.FluentApi.GetContext())
      using (var fileOptions = Factory.GetExcelFileOptions())
      using (var repo = new EPPlusDomainRepository<DomainObject_FluentApi, int>(context, fileOptions)) {
        var parsedRow10 = repo.GetByRowId(10);
        UnitTestMetadata.AssertEqual(UnitTestMetadata.ActualRow10, parsedRow10);

        parsedRow10 = repo.GetByKey(10);
        UnitTestMetadata.AssertEqual(UnitTestMetadata.ActualRow10, parsedRow10);
      }
    }


    [Fact]
    public void SetFileLocationAtLaterTime() {
      using (var context = Factory.FluentApi.GetContext()) {
        using (var fileOptions = Factory.GetExcelFileOptions()) {

          //remove all file related info from options
          var fileStream = fileOptions.ExcelFileStream;
          fileOptions.ExcelFileStream = null;

          var worksheetName = fileOptions.WorksheetName;
          fileOptions.WorksheetName = null;

          var filePassword = fileOptions.FilePassword;
          fileOptions.FilePassword = null;

          using (var repo = new EPPlusDomainRepository<DomainObject_FluentApi, int>(context, fileOptions)) {
            DomainObject_FluentApi parsedRow;

            //without file information, repo runs into exception
            Assert.Throws<NullReferenceException>(() => {
              parsedRow = repo.GetByRowId(10);
            });

            //set file location before usage
            repo.SetExcelDataSource(fileStream, worksheetName, filePassword);
            parsedRow = repo.GetByRowId(10);
            UnitTestMetadata.AssertEqual(UnitTestMetadata.ActualRow10, parsedRow);

          }
        }
      }
    }
  }
}
