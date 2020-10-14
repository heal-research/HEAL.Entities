using System.Linq;
using HEAL.Entities.DataAccess.EPPlus.ContextConfiguration.Tests;
using HEAL.Entities.DataAccess.EPPlus.Tests;
using Xunit;

namespace HEAL.Entities.DataAccess.EPPlus {
  public class UnitTestMetadata {
    public const int DelimitedExcelRepo_MaxmimumLineNumber = 11;
    public const int ExcelDataCount= 20;
    public const string DefaultOccupation= "UNEMPLOYED";

    public static void AssertEqual(ITestDomainObject expected, ITestDomainObject actual) {
      Assert.Equal(expected.Age, actual.Age);
      Assert.Equal(expected.PrimaryKey, actual.PrimaryKey);
      Assert.Equal(expected.HasChildren, actual.HasChildren);
      Assert.Equal(expected.Married, actual.Married);
      Assert.Equal(expected.Occupation, actual.Occupation);
      Assert.Equal(expected.Prename, actual.Prename);
      Assert.Equal(expected.Salary, actual.Salary);
      Assert.Equal(expected.Surname, actual.Surname);
      Assert.Equal(expected.Hobbies?.OrderBy(x=>x), actual.Hobbies?.OrderBy(x => x));
    }

    public static DomainObject_FluentApi ActualRow10 = new DomainObject_FluentApi() {
      Age = 23,
      HasChildren = true,
      Married = true,
      Occupation = UnitTestMetadata.DefaultOccupation,
      Prename = "Tess",
      Surname = "Morrison",
      Salary = 88.44m,
      Hobbies = new string[]{
          "Videography",
          "Photography",
          "Drone Pilot"
        },
      PrimaryKey = 10,
    };
  }
}
