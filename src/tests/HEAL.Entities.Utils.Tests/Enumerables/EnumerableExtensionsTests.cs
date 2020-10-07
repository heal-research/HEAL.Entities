using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using HEAL.Entities.Utils.Data;

namespace HEAL.Entities.Utils.Enumerables.Tests {
  public class EnumerableExtensionsTests {

    public class Person {
      public string Name { get; set; }
      public int Age { get; set; }
    }

    public static Person p1 = new Person() { Name = "Peter", Age = 26 };
    public static Person p2 = new Person() { Name = "Alex", Age = 54 };
    public static Person p3 = new Person() { Name = "Ann", Age = 34 };
    public static Person p4 = new Person() { Name = "Mary", Age = 57 };
    public static Person p5 = new Person() { Name = "Herbert", Age = 45 };
    public static Person[] persons = new Person[] { p1, p2, p3, p4, p5 };

    [Fact]
    public void MinMaxOrDefaultByTests() {
      var emptyList = new List<Person>();
      var expected = emptyList.Where(x => x.Age == emptyList.Max(y => y.Age)).FirstOrDefault();
      var actual = emptyList.MaxOrDefaultBy(x => x.Age);

      Assert.Equal(expected, actual);
    }


    [Fact]
    public void MaxOrDefaultByTests() {
      var expected = persons.Where(x => x.Age == persons.Max(y => y.Age)).FirstOrDefault();
      var actual = persons.MaxOrDefaultBy(x => x.Age);

      Assert.Equal(expected, actual);
    }

    [Fact]
    public void MinOrDefaultByTests() {
      var expected = persons.Where(x => x.Age == persons.Min(y => y.Age)).FirstOrDefault();
      var actual = persons.MinOrDefaultBy(x => x.Age);

      Assert.Equal(expected, actual);
    }

    [Fact]
    public void OrderByExampleCanUseDefaultComparator() {
      var example = new List<string>() { "b", "a", "c" };
      var items = new List<string>() { "a", "a", "b", "b", "c", "c" };
      var result = new List<string>() { "b", "b", "a", "a", "c", "c" };

      var ordered = items.OrderByExample(example).ToList();

      Assert.Equal(result, ordered);
    }

    [Fact]
    public void OrderByExampleCustomComparator() {
      var exampleWithWildCards = new List<string>() { "b*", "a?", "c*" };
      var items = new List<string>() { "ab", "ac", "ba", "bc", "ca", "cb" };
      var result = new List<string>() { "ba", "bc", "ab", "ac", "ca", "cb" };

      var ordered = items.OrderByExample(exampleWithWildCards, WildCardPatternComparator).ToList();

      Assert.Equal(result, ordered);
    }

    [Fact]
    public void ItemsThatAreNotMatchedAreStillReturnedInOriginalOrder() {
      var exampleWithWildCards = new List<string>() { "b*", "b*", "c*" };
      var items = new List<string>() { "ba", "bc", "ca", "cb", "xx", "zz", "yy" };
      var result = new List<string>() { "ba", "bc", "ca", "cb", "xx", "zz", "yy" };

      var ordered = items.OrderByExample(exampleWithWildCards, WildCardPatternComparator).ToList();

      Assert.Equal(result, ordered);
    }

    [Fact]
    public void MultipleMatchesDoNotCauseMoreItems() {
      var exampleWithWildCards = new List<string>() { "b*", "b*" };
      var items = new List<string>() { "ba", "bc" };
      var result = new List<string>() { "ba", "bc" };

      var ordered = items.OrderByExample(exampleWithWildCards, WildCardPatternComparator).ToList();

      Assert.Equal(result, ordered);
    }

    [Fact]
    public void MoreThanOnePatternsMatchingCausePriorizedOrder() {
      var exampleWithWildCards = new List<string>() { "b*", "a?", "a*", "c*" };
      var items = new List<string>() { "abd", "ac" /*is matched by a* and a? but a? is higher priority -> is returned first and ONLY ONCE although matched 2 times*/ , "ba", "bc", "ca", "cb" };
      var result = new List<string>() { "ba", "bc", "ac", "abd", "ca", "cb" };

      var ordered = items.OrderByExample(exampleWithWildCards, WildCardPatternComparator).ToList();

      Assert.Equal(result, ordered);
    }

    private bool WildCardPatternComparator(string pattern, string item) {
      return Regex.IsMatch(item, pattern.WildCardToRegex());
    }


    [Fact]
    public void WithoutTest() {

      var left = new List<string>() { "abd", "ac", "ba", "bc", "ca", "cb" };
      var right = new List<string>() { "ba", "ca", };
      var result = new List<string>() { "abd", "ac", "bc", "cb" };

      var withouth = left.Without(right).ToList();

      Assert.Equal(result, withouth);
    }
  }
}
