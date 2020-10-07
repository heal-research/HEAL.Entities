using HEAL.Entities.Objects;

namespace HEAL.Entities.DataAccess.EPPlus.Tests {
  /// <summary>
  /// common interface for the data structure so that unit test code can be centralized and compared over the two configuration variants
  /// </summary>
  public interface ITestDomainObject : IDomainObject<int> {
    string Prename { get; set; }
    string Surname { get; set; }
    int Age { get; set; }
    string Occupation { get; set; }
    decimal Salary { get; set; }
    bool Married { get; set; }
    bool HasChildren { get; set; }
    string[] Hobbies { get; set; }
  }
}
