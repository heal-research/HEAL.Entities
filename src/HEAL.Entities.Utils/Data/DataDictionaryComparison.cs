using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HEAL.Entities.Utils.Data {

  /// <summary>
  /// Used to identify which type of change was done on manipulated datasets
  /// </summary>
  public enum ChangeType {
    Unchanged, Added, Deleted, Modified
  }


  public static class DataDictionaryComparison {

    /// <summary>
    /// returns true if both data dictionaries store the same data
    /// </summary>
    /// <param name="expected">base data</param>
    /// <param name="actual">changed data</param>
    /// <returns></returns>
    public static bool CompareData(this IDictionary<DataDictionaryColumn, IList> expected,
                                   IDictionary<DataDictionaryColumn, IList> actual,
                                   DataDictionaryComparisonConfiguration config = default) {
      _ = CompareData(expected, actual, out _, out bool equal, config);
      return equal;
    }

    /// <summary>
    /// returns true if both data dictionaries store the same data 
    /// provides a detailed (jagged) [columns]*[rows] <paramref name="changeMatrix"/> of changes (i.e. a diff)
    /// the string array <paramref name="columnOrder"/> determines the row order of the change matrix
    /// </summary>
    /// <param name="expected">base data</param>
    /// <param name="actual">changed data</param>
    /// <param name="columnOrder">provides the used row order of the change matrix</param>
    /// <param name="changeMatrix">(jagged) [columns]*[rows] <paramref name="changeMatrix"/> of changes</param>
    /// <returns></returns>
    public static bool CompareData(this IDictionary<DataDictionaryColumn, IList> expected,
                                   IDictionary<DataDictionaryColumn, IList> actual,
                                   out string[] columnOrder,
                                   out ChangeType[][] changeMatrix,
                                   DataDictionaryComparisonConfiguration config = default) {
      changeMatrix = CompareData(expected, actual, out columnOrder, out bool equal, config);
      return equal;
    }

    /// <summary>
    /// returns a detailed (jagged) [columns]*[rows] <paramref name="changeMatrix"/> of changes (i.e. a diff)
    /// the string array <paramref name="columnOrder"/> determines the row order of the change matrix
    /// </summary>
    /// <param name="expected">base data</param>
    /// <param name="actual">changed data</param>
    /// <param name="columnOrder">provides the used row order of the change matrix</param>
    /// <param name="equal"></param>
    /// <returns></returns>
    public static ChangeType[][] CompareData(this IDictionary<DataDictionaryColumn, IList> expected,
                                             IDictionary<DataDictionaryColumn, IList> actual,
                                             out string[] columnOrder,
                                             out bool equal,
                                             DataDictionaryComparisonConfiguration config = default) {
      equal = true;
      var expectedColumns = expected.Keys;
      var actualColumns = actual.Keys;
      columnOrder = expectedColumns.Select(x => x.Name).Union(actualColumns.Select(x => x.Name)).Distinct().OrderBy(x => x).ToArray();
      var maxColumns = columnOrder.Count();

      ChangeType[][] change = new ChangeType[maxColumns][];
      var columnIndex = 0;

      foreach (var column in columnOrder) {
        if (expectedColumns.ContainsVariable(column) && actualColumns.ContainsVariable(column)) {
          var maxRows = Math.Max(expected.GetValues(column).Count, actual.GetValues(column).Count);
          change[columnIndex] = new ChangeType[maxRows];

          var valueType = expected.GetValueType(column);
          if (valueType != actual.GetValueType(column)) {
            //type got changed -> all modified
            for (int i = 0; i < maxRows; i++) {
              change[columnIndex][i] = ChangeType.Modified;
              equal = false;
            }
          } else {
            object[] parameters = new object[] { expected.GetValues(column), actual.GetValues(column), config };
            var genericComparison = typeof(DataDictionaryComparison).GetMethodWithLinq(nameof(CompareData), typeof(IList), typeof(IList), typeof(DataDictionaryComparisonConfiguration))
                             .MakeGenericMethod(valueType);

            change[columnIndex] = (ChangeType[])genericComparison.Invoke(null, parameters);
            //column available in both -> check data
            equal = equal && change[columnIndex].All(x => x == ChangeType.Unchanged);
          }
        } else if (expectedColumns.ContainsVariable(column) && !actualColumns.ContainsVariable(column)) {
          var maxRows = expected.GetValues(column).Count;
          change[columnIndex] = new ChangeType[maxRows];

          //column expected, not available anymore-> got deleted
          for (int i = 0; i < maxRows; i++) {
            change[columnIndex][i] = ChangeType.Deleted;
          }
          equal = false;
        } else if (!expectedColumns.ContainsVariable(column) && actualColumns.ContainsVariable(column)) {
          var maxRows = actual.GetValues(column).Count;
          change[columnIndex] = new ChangeType[maxRows];

          //column not expected but found -> got added
          for (int i = 0; i < maxRows; i++) {
            change[columnIndex][i] = ChangeType.Added;
          }
          equal = false;
        }
        columnIndex++;
      }

      return change;
    }

    public static MethodInfo GetMethodWithLinq(this Type staticType, string methodName,
    params Type[] paramTypes) {
      var methods = from method in staticType.GetMethods()
                    where method.Name == methodName
                          && method.GetParameters()
                                   .Select(parameter => parameter.ParameterType)
                                   .Select(type => type.IsGenericType ?
                                       type.GetGenericTypeDefinition() : type)
                                   .SequenceEqual(paramTypes)
                    select method;
      try {
        return methods.SingleOrDefault();
      }
      catch (InvalidOperationException) {
        throw new AmbiguousMatchException();
      }
    }

    /// <summary>
    /// returns true if both data lists store the same data
    /// provides a detailed array <paramref name="changeArray"/>[rows] of changes (i.e. a diff)
    /// </summary>
    /// <typeparam name="T">type of value stored in the lists, must implement <see cref="IComparable{T}"/></typeparam>
    /// <param name="expectedData">base data</param>
    /// <param name="actualData">changed data</param>
    /// <param name="equal"></param>
    /// <returns></returns>
    public static bool CompareData<T>(this IList expectedData, IList actualData, out ChangeType[] changeArray, DataDictionaryComparisonConfiguration config = default) {
      return CompareData<T>(expectedData as IList<T>, actualData as IList<T>, out changeArray, config);
    }

    /// <summary>
    /// returns true if both data lists store the same data
    /// provides a detailed array <paramref name="changeArray"/>[rows] of changes (i.e. a diff)
    /// </summary>
    /// <typeparam name="T">type of value stored in the lists, must implement <see cref="IComparable{T}"/></typeparam>
    /// <param name="expectedData">base data</param>
    /// <param name="actualData">changed data</param>
    /// <param name="equal"></param>
    /// <returns></returns>
    public static bool CompareData<T>(this IList<T> expectedData, IList<T> actualData, out ChangeType[] changeArray, DataDictionaryComparisonConfiguration config = default) {
      changeArray = CompareData(expectedData, actualData, out bool equal, config);
      return equal;
    }


    /// <summary>
    /// provides a detailed array <paramref name="change"/>[rows] of changes (i.e. a diff)
    /// </summary>
    /// <typeparam name="T">type of value stored in the lists, must implement <see cref="IComparable{T}"/></typeparam>
    /// <param name="expectedData">base data</param>
    /// <param name="actualData">changed data</param>
    /// <param name="equal"></param>
    /// <returns></returns>
    public static ChangeType[] CompareData<T>(this IList expectedData, IList actualData, DataDictionaryComparisonConfiguration config = default) {
      return CompareData<T>(expectedData as IList<T>, actualData as IList<T>, out bool _, config);
    }

    /// <summary>
    /// provides a detailed array <paramref name="change"/>[rows] of changes (i.e. a diff)
    /// </summary>
    /// <typeparam name="T">type of value stored in the lists, must implement <see cref="IComparable{T}"/></typeparam>
    /// <param name="expectedData">base data</param>
    /// <param name="actualData">changed data</param>
    /// <param name="equal"></param>
    /// <returns></returns>
    public static ChangeType[] CompareData<T>(this IList expectedData, IList actualData, out bool equal, DataDictionaryComparisonConfiguration config = default) {
      return CompareData<T>(expectedData as IList<T>, actualData as IList<T>, out equal, config);
    }


    /// <summary>
    /// provides a detailed array <paramref name="change"/>[rows] of changes (i.e. a diff)
    /// </summary>
    /// <typeparam name="T">type of value stored in the lists, must implement <see cref="IComparable{T}"/></typeparam>
    /// <param name="expectedData">base data</param>
    /// <param name="actualData">changed data</param>
    /// <param name="equal"></param>
    /// <returns></returns>
    public static ChangeType[] CompareData<T>(this IList<T> expectedData, IList<T> actualData, out bool equal, DataDictionaryComparisonConfiguration config = default) {
      var changeArray = new ChangeType[Math.Max(expectedData.Count, actualData.Count)];
      equal = true;
      //jagged data lists?
      var comparisonLength = Math.Min(expectedData.Count, actualData.Count);
      if (comparisonLength < expectedData.Count) {
        //rows got deleted
        equal = false;
        for (int i = comparisonLength; i < expectedData.Count; i++) {
          changeArray[i] = ChangeType.Deleted;
        }
      }
      if (comparisonLength < actualData.Count) {
        //rows got added
        equal = false;
        for (int i = comparisonLength; i < actualData.Count; i++) {
          changeArray[i] = ChangeType.Added;
        }
      }
      for (int i = 0; i < comparisonLength; i++) {
        if (config != default && config.DecimalPrecision.HasValue && IsDecimalOrFloat<T>()) {
          if (!CompareDecimalAndFloat<T>(expectedData[i], actualData[i], config)) {
            equal = false;
            changeArray[i] = ChangeType.Modified;
          }
        } else if (Comparer<T>.Default.Compare(expectedData[i], actualData[i]) != 0) {
          equal = false;
          changeArray[i] = ChangeType.Modified;
        }
      }
      return changeArray;
    }
    private static bool IsDecimalOrFloat<T>() {
      if (typeof(T) == typeof(double?)
        || typeof(T) == typeof(Single?)
        || typeof(T) == typeof(decimal?)
        || typeof(T) == typeof(double)
        || typeof(T) == typeof(Single)
        || typeof(T) == typeof(decimal))
        return true;

      return false;
    }

    private static bool CompareDecimalAndFloat<T>(T t1, T t2, DataDictionaryComparisonConfiguration config = default) {
      if (t1 == null && t2 == null) return true;
      if (!(t1 != null && t2 != null)) return false;

      if (!(t1 is IConvertible t1Convertible)) throw new NotSupportedException($"Type of value '{typeof(T)}' is not supported by method '{nameof(CompareDecimalAndFloat)}'.");
      if (!(t2 is IConvertible t2Convertible)) throw new NotSupportedException($"Type of value '{typeof(T)}' is not supported by method '{nameof(CompareDecimalAndFloat)}'.");
      return AlmostEqual(t1Convertible.ToDecimal(CultureInfo.InvariantCulture), t2Convertible.ToDecimal(CultureInfo.InvariantCulture), config);
    }

    private static bool AlmostEqual(decimal t1, decimal t2, DataDictionaryComparisonConfiguration config = default) {
      return (System.Math.Round(t1 - t2, config.DecimalPrecision ?? int.MaxValue) == 0);
    }
  }
}
