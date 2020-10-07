using System;

namespace HEAL.Entities.Objects {
  /// <summary>
  /// Implement to provide base domain object functionality. Whereas primary key type can be composite. 
  /// </summary>
  /// <typeparam name="T">type of primary key. use struct for composite keys</typeparam>
  public interface IDomainObject<T> : IComparable<IDomainObject<T>>
                    where T : IComparable<T> {
    T PrimaryKey { get; set; }
  }
}