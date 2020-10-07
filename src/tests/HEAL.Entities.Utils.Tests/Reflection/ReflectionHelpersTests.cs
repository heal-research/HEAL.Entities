using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using HEAL.Entities.Utils.Data;
using HEAL.Entities.Utils.Reflection;

namespace HEAL.Entities.Utils.Enumerables.Tests {


  public class ReflectionHelpersTests {
    public int TestProperty { get; set; }


    [Fact]
    public void GetPropertyInfoByExpression() {

      var actualPropertyInfo = typeof(ReflectionHelpersTests).GetProperty(nameof(TestProperty));

      var expressionPropertyInfo = ReflectionHelpers.GetPropertyInfo<ReflectionHelpersTests, int>(x => x.TestProperty);

      Assert.Equal(actualPropertyInfo.Name, expressionPropertyInfo.Name);
      Assert.Equal(actualPropertyInfo.PropertyType, expressionPropertyInfo.PropertyType);
    }

  }
}
