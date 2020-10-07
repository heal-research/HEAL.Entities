using System;
using HEAL.Entities.DataAccess.EPPlus.Tests;
using HEAL.Entities.Objects;
using HEAL.Entities.Objects.Excel;

namespace HEAL.Entities.DataAccess.EPPlus.AttributeConfiguration.Tests {

  /// <summary>
  /// a test domain object with <see cref="ExcelColumnConfigurationAttribute"/>s applied
  /// disadvantage: DomainObject has excel specific information that is not really necessary
  /// </summary>
  public class DomainObject_Attributes : IDomainObject<int>, ITestDomainObject {
    [ExcelAudit(ExcelAuditProperties.RowId)]
    public int PrimaryKey { get; set; }  
    
    [ExcelColumnConfiguration(ExcelColumnEnum.A, "First Name")]
    public string Prename { get; set; }
    [ExcelColumnConfiguration(2,"Last Name")]
    public string Surname { get; set; }
    [ExcelColumnConfiguration("C","Age")]
    public int Age { get; set; }
    [ExcelColumnConfiguration(ExcelColumnEnum.D, DefaultValue: UnitTestMetadata.DefaultOccupation)]
    public string Occupation { get; set; }
    [ExcelColumnConfiguration(ExcelColumnEnum.E)]
    public decimal Salary { get; set; }
    [ExcelColumnConfiguration(ExcelColumnEnum.F, "Marital Status", cellParser: typeof(MaritalStatusParser))]
    public bool Married { get; set; }
    [ExcelColumnConfiguration(ExcelColumnEnum.G, "Has Children", cellParser: typeof(BinaryBooleanParser))]
    public bool HasChildren { get; set; }
    [ExcelColumnConfiguration(ExcelColumnEnum.H, cellParser: typeof(CommaSeparatedListParser))]
    public string[] Hobbies { get; set; }

    public int CompareTo(IDomainObject<int> other) {
      throw new NotImplementedException();
    }
  }
}
