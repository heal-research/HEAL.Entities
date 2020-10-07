using System;

namespace HEAL.Entities.Objects {
  /// <summary>
  /// used to describe the unit that are stored in a specific property
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class UnitAttribute : Attribute {
    public UnitAttribute(string UnitString) {
      this.UnitString = UnitString;
    }
    public string UnitString { get; private set; }
  }
}
