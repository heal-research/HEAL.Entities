
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HEAL.Entities.Utils.Data {
  public static class RegexExtensions {
    /// <summary>
    /// Converts a WildCard search string containing "*" and "?" to a regular expression pattern
    /// </summary>
    /// <param name="value">WildCard search string</param>
    /// <returns>Regex Patern</returns>
    public static String WildCardToRegex(this String value) {
      return "^" + Regex.Escape(value).Replace($"\\?", ".").Replace($"\\*", ".*") + "$";
    }
  }
}
