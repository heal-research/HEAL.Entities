using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace HEAL.Entities.Utils.Data.Tests {
  public class RegexExtensionsTests {
    [Fact]
    public void WildCartToRegex() {
      var regex = new Regex("abc?e*jk".WildCardToRegex());
      Assert.Matches(regex, "abcdefghijk");
      Assert.DoesNotMatch(regex, "abcddefghijk"); //only 1 chracter between 'c' and 'e' allowed
      Assert.DoesNotMatch(regex, "abcdefghijkaaaa"); //no multiple characters at end
    }
  }
}
