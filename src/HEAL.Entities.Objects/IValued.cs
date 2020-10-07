namespace HEAL.Entities.Objects {

  /// <summary>
  /// Indicates that an object has a property named Value of type <typeparamref name="T"/>
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IValued<T> {
    T Value { get; set; }
  }
}