using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HEAL.Entities.Utils.Data {

    /// <summary>
    /// Type used to specify columns in DataDictionary of <see cref="IDictionary{TKey, TValue}"/> where key is <see cref="DataDictionaryColumn"/>
    /// This namespace provides additional extension methods for such dictionaries. <see cref="DataDictionaryColumn.CreateColumn{T}(string)"/> to create an instance or use
    /// <see cref="DataDictionaryExtensions.Add{T}(IDictionary{DataDictionaryColumn, IList}, IList{T}, string)"/> which includes this call
    /// </summary>
  public class DataDictionaryColumn {
    private DataDictionaryColumn() {

    }

    public DataDictionaryColumn(string columnName, Type dataType) {
      this.Name = columnName;
      this.Type = dataType;
    }

    public static DataDictionaryColumn CreateColumn<T>(string columnName){
      var column = new DataDictionaryColumn();
      column.Type = typeof(T);
      column.Name = columnName;
      return column;
    }

    public override bool Equals(object obj) {
      return obj is DataDictionaryColumn column &&
             Name == column.Name &&
             EqualityComparer<Type>.Default.Equals(Type, column.Type);
    }

    public override int GetHashCode() {
      int hashCode = -243844509;
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
      hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(Type);
      return hashCode;
    }

    public string Name { get; private set; }
    //public DataDictionarySupportedValueTypes DataTypeEnum { get; private set; }
    public Type Type { get; private set; } 
  }

  public static class DataDictionaryExtensions {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public static void Add<T>(this IDictionary<DataDictionaryColumn, IList> data, IList<T> list, string columnName) {
      if (data.Keys.Where(x => x.Name == columnName).Count() > 0) throw new ArgumentException($"This dictionary already contains a column named '{columnName}'");
      data.Add(DataDictionaryColumn.CreateColumn<T>(columnName), list as IList);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public static bool ContainsVariable(this IDictionary<DataDictionaryColumn, IList> data, string variableName) {
      return data.Keys.ContainsVariable(variableName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public static bool ContainsVariable(this IEnumerable<DataDictionaryColumn> columns, string variableName) {
      return columns.Where(x => x.Name == variableName).Count() == 1;
    }

    /// <summary>
    /// Returns list of values for given variable
    /// Requires provided type for data stored in the list. Use <see cref="GetValueType(string)"/> to obtain information about the stored value type.
    /// </summary>
    /// <typeparam name="T">Type of data stored in the List</typeparam>
    /// <param name="variableName">name of the variable</param>
    /// <returns>generic cast of stored list</returns>
    public static IList<T> GetValues<T>(this IDictionary<DataDictionaryColumn, IList> data, string variableName) {
      if (!(data.GetValues(variableName) is IList<T> values)) throw new ArgumentException("The variable " + variableName + " cannot be cast to 'IList<" + typeof(T) + ">'.");
      return values;
    }

    /// <summary>
    /// Returns list of values for given variable
    /// Requires provided type for data stored in the list. Use <see cref="GetValueType(string)"/> to obtain information about the stored value type.
    /// </summary>
    /// <typeparam name="T">Type of data stored in the List</typeparam>
    /// <param name="variableName">name of the variable</param>
    /// <returns>generic cast of stored list</returns>
    public static IList GetValues(this  IDictionary<DataDictionaryColumn, IList> data, string variableName) {
      return data[data.Keys.Where(x => x.Name == variableName).Single()];
    }

    /// <summary>
    /// returns true if <see cref="IList"/> of values can be cast to <see cref="IList{T}"/>
    /// </summary>
    /// <typeparam name="T">target value type</typeparam>
    /// <param name="data"></param>
    /// <param name="variableName">name of the variable</param>
    /// <returns></returns>
    public static bool VariableHasType<T>(this  IDictionary<DataDictionaryColumn, IList> data, string variableName) {
      return data.Keys.Where(x => x.Name == variableName).Single().Type == typeof(T);
    }

    /// <summary>
    /// returns true if <see cref="IList"/> of values can be cast to <see cref="IList{T}"/>
    /// </summary>
    /// <typeparam name="T">target value type</typeparam>
    /// <param name="data"></param>
    /// <param name="variableName">name of the variable</param>
    /// <returns></returns>
    public static bool VariableHasType(this  IDictionary<DataDictionaryColumn, IList> data, string variableName, Type type) {
      return data.Keys.Where(x => x.Name == variableName).Single().Type ==type;
    }

    /// <summary>
    /// returns type of data stored in identified variable
    /// </summary>
    /// <param name="variableName">name of the variable</param>
    /// <returns></returns>
    public static Type GetValueType(this  IDictionary<DataDictionaryColumn, IList> data, string variableName) {
      return data.Keys.Where(x => x.Name == variableName).Single().Type; 
    }

    /// <summary>
    /// returns single cell value identified
    /// Requires provided type for data stored in the list. Use <see cref="GetValueType(string)"/> to obtain information about the stored value type.
    /// </summary>
    /// <typeparam name="T">Type of data stored in the List</typeparam>
    /// <param name="variableName">name of the variable</param>
    /// <param name="row"></param>
    /// <returns>single value</returns>
    public static T GetValue<T>(this  IDictionary<DataDictionaryColumn, IList> data, string variableName, int row) {
      var values = data.GetValues<T>(variableName);
      return values.ElementAt(row);
    }

    /// <summary>
    /// returns number of values stored for this variable
    /// </summary>
    /// <param name="variableName">name of the variable</param>
    /// <returns></returns>
    public static int VariableRowCount(this  IDictionary<DataDictionaryColumn, IList> data, string variableName) {
      return data[data.Keys.Where(x => x.Name == variableName).Single()].Count;
    }

    /// <summary>
    /// returns the lowest number of values stored for any variable
    /// </summary>
    /// <param name="variableName">name of the variable</param>
    /// <returns></returns>
    public static int CountLowestDimension(this  IDictionary<DataDictionaryColumn, IList> data) {
      return data.Min(x => x.Value.Count);
    }
    /// <summary>
    /// returns the lowest number of values stored for any variable
    /// </summary>
    /// <param name="variableName">name of the variable</param>
    /// <returns></returns>
    public static int CountHighestDimension(this  IDictionary<DataDictionaryColumn, IList> data) {
      return data.Max(x => x.Value.Count);
    }
  }

}
