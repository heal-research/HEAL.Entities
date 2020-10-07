using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace HEAL.Entities.Utils.Data.Tests {
  public class QuickEqualityComparerTests {
    [Fact]
    public void CustomStringComparison() {
      var left = new List<string>() { "b", "a", "c" };
      var right = new List<string>() { "b1", "a2", "c3" };

      //provide an arbitrary function that compares the two individuals
      Assert.True(left.SequenceEqual(right, 
                        new QuickEqualityComparer<string>((first, second) => { return second.StartsWith(first); })));
    }

  }
}
