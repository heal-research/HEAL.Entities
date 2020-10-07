
using System;
using System.Collections.Generic;
using System.Text;

namespace HEAL.Entities.Utils.Data {

  /// <summary>
  /// create an implementation of <see cref="IEqualityComparer{T}"/> by providing a 
  /// comparator function of signature <see cref="Func{T, T, bool}"/> where T1 and T2 are of <typeparamref name="T"/> and TResult is of type <see cref="bool"/> <br/>
  /// allows for example to compare persons once by age and another time by social ID or name without having to change class implementation
  /// </summary>
  /// <typeparam name="T">type to be compared</typeparam>
  public class QuickEqualityComparer<T> : IEqualityComparer<T> {
    Func<T, T, bool> comparer;

    public QuickEqualityComparer(Func<T, T, bool> comparer) {
      this.comparer = comparer;
    }

    public bool Equals(T a, T b) {
      return comparer(a, b);
    }

    public int GetHashCode(T a) {
      return a.GetHashCode();
    }
  }
}
