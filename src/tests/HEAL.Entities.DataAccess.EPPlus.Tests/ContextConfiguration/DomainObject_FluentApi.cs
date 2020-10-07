using System;
using HEAL.Entities.DataAccess.EPPlus.Tests;
using HEAL.Entities.Objects;

namespace HEAL.Entities.DataAccess.EPPlus.ContextConfiguration.Tests {

  public class DomainObject_FluentApi : IDomainObject<int>, ITestDomainObject {
    public int PrimaryKey { get; set; }

    public string Prename { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
    public string Occupation { get; set; }
    public decimal Salary { get; set; }
    public bool Married { get; set; }
    public bool HasChildren { get; set; }
    public string[] Hobbies { get; set; }

    public int CompareTo(IDomainObject<int> other) {
      throw new NotImplementedException();
    }
  }
}
