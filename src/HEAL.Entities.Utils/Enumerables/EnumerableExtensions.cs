using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace HEAL.Entities.Utils.Enumerables {
  public static class EnumerableExtensions {

    /// <summary>
    /// Extracts the first entry of a generic list, where one property has the max value of all other entries
    /// e.g. first Person in List\<Person\> where age is max
    /// </summary>
    /// <typeparam name="TEntity">Type of entities in Enumerable</typeparam>
    /// <typeparam name="TProperty">Type of target property of  <typeparam name="TEntity"></typeparam>
    /// <param name="source">List where first Max of property must be found</param>
    /// <param name="propertyExpression">expression to extract <typeparam name="TProperty"> of <typeparam name="TEntity"></param>
    /// <returns></returns>
    public static TEntity MaxOrDefaultBy<TEntity, TProperty>(this IEnumerable<TEntity> source, Expression<Func<TEntity, TProperty>> propertyExpression)
            where TProperty : IComparable<TProperty> {
      if (source == null)
        return default(TEntity);

      TEntity max = source.FirstOrDefault();
      var expression = propertyExpression.Compile();
      foreach (var item in source.Skip(1)) {
        if (expression(max).CompareTo(expression(item)) < 0) {
          max = item;
        }
      }
      return max;
    }

    /// <summary>
    /// Extracts the first entry of a generic list, where one property has the min value of all other entries
    /// /// e.g. first Person in List\<Person\> where age is min
    /// </summary>
    /// <typeparam name="TEntity">Type of entities in Enumerable</typeparam>
    /// <typeparam name="TProperty">Type of target property of  <typeparam name="TEntity"></typeparam>
    /// <param name="source">List where first Min of property must be found</param>
    /// <param name="propertyExpression">expression to extract <typeparam name="TProperty"> of <typeparam name="TEntity"></param>
    /// <returns></returns>
    public static TEntity MinOrDefaultBy<TEntity, TProperty>(this IEnumerable<TEntity> source, Expression<Func<TEntity, TProperty>> propertyExpression)
            where TProperty : IComparable<TProperty> {
      if (source == null)
        return default(TEntity);

      TEntity min = source.FirstOrDefault();
      var expression = propertyExpression.Compile();
      foreach (var item in source.Skip(1)) {
        if (expression(min).CompareTo(expression(item)) > 0) {
          min = item;
        }
      }
      return min;
    }


    /// <summary>
    /// returns a new IEnumerable with only elements of <paramref name="left"/> not beeing present in <paramref name="right"/>
    /// </summary>
    /// <typeparam name="T">Typ must implement IComparable</typeparam>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static IEnumerable<T> Without<T>(this IEnumerable<T> left, IEnumerable<T> right)
        where T : IComparable<T> {
      foreach (var item in left) {
        if (!right.Contains(item))
          yield return item;
      }
    }

    /// <summary>
    /// Orders the source enumerable by matching example values with the defined comparator. Not matched items are appended in original order.
    /// source:{"y","x","a","b","b"}, example:{"b","a"} => {"b","b","a","y","x",}  
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="exampleEnumerable"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> OrderByExample<TSource, TExample>(this IEnumerable<TSource> source, IEnumerable<TExample> exampleEnumerable)
        where TExample : IComparable<TSource> {
      return OrderByExample<TSource, TExample>(source, exampleEnumerable, (example, item) => { return example.CompareTo(item) == 0; });
    }

    /// <summary>
    /// Orders the source enumerable by matching example values with the given comparator. Not matched items are appended in original order.
    /// source:{"y","x","a","b","b"}, example:{"b","a"} => {"b","b","a","y","x",}  
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="exampleEnumerable"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> OrderByExample<TSource, TExample>(this IEnumerable<TSource> source, IEnumerable<TExample> exampleEnumerable, Func<TExample, TSource, bool> matchPredicate) {
      var sourceItems = new List<TSource>(source);
      var result = new List<TSource>();
      foreach (var example in exampleEnumerable) {
        foreach (var match in sourceItems.Where(item => matchPredicate(example, item)).ToList()) {
          sourceItems.Remove(match);
          result.Add(match);
        }
      }
      result.AddRange(sourceItems); //add remaining items which were not matched in original order
      return result;
    }


    /// <summary>
    /// counts the occurance of each item in an ienumerable <br/>
    /// source:["a","b","a","b","b"], result [{"a",2},{"b",3}}
    /// </summary>
    /// <typeparam name="TValue">value type</typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IDictionary<TValue,int> CountDistinct<TValue>(this IEnumerable<TValue> source) {
      return
        source
        .GroupBy(key => key)
        .Select(g => new {
          Key = g.Key,
          Count = g.Count()
        }).ToDictionary(x => x.Key, x => x.Count);
    }
  }
}
