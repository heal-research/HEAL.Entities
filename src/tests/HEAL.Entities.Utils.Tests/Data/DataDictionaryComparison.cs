using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Collections;

using ct = HEAL.Entities.Utils.Data.ChangeType;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace HEAL.Entities.Utils.Data.Tests {
  public class DataDictionaryComparison {
    private static DateTime time = DateTime.Now;
    private static  IDictionary<DataDictionaryColumn, IList> data1 = new Dictionary<DataDictionaryColumn, IList>() {
       {DataDictionaryColumn.CreateColumn<double>("a"), new List<double>{ 1,2,3,4,5,6,7,8,9} }
      ,{DataDictionaryColumn.CreateColumn<double>("b"), new List<double>{ 1,2,3,4,5,6,7,8,9} }
      ,{DataDictionaryColumn.CreateColumn<string>("c"), new List<string>{"a", "b", "c", "d", "e", "f", "g", "h", "i"} }
      ,{DataDictionaryColumn.CreateColumn<DateTime>("d"), new List<DateTime>{ time.AddSeconds(1)
                                , time.AddSeconds(2)
                                , time.AddSeconds(3)
                                , time.AddSeconds(4)
                                , time.AddSeconds(5)
                                , time.AddSeconds(6)
                                , time.AddSeconds(7)
                                , time.AddSeconds(8)
                                , time.AddSeconds(9) } }
    };
    private static  IDictionary<DataDictionaryColumn, IList> data2 = new Dictionary<DataDictionaryColumn, IList>() {
       {DataDictionaryColumn.CreateColumn<double>("a"), new List<double>{ 1,2,3,4,5,6,7,8,9} }
      ,{DataDictionaryColumn.CreateColumn<string>("c"), new List<string>{"a", "b", "c", "d", "e", "f", "g", } }
      ,{DataDictionaryColumn.CreateColumn<DateTime>("d"), new List<DateTime>{ time.AddSeconds(1)
                                , time.AddSeconds(2)
                                , time.AddSeconds(3)
                                , time.AddSeconds(4)
                                , time.AddSeconds(5)
                                , time.AddSeconds(60)
                                , time.AddSeconds(70)
                                , time.AddSeconds(80)
                                , time.AddSeconds(90)
                                , time.AddSeconds(90)} }
      ,{DataDictionaryColumn.CreateColumn<double>("e"), new List<double>{ 1,2,3,4,5,6,7,8,9} }
    };
    private static string[] expected_columnOrder = new string[5] { "a", "b", "c", "d", "e" };
    private static ct[][] expected_Diff_data1_to_data2 = new ct[5][] {
                     /* 0               1                2               3               4                5              6              7              8              9    */
      /*a*/new ct[9 ]{ ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  },
      /*b*/new ct[9 ]{ ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    },
      /*c*/new ct[9 ]{ ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Deleted    , ct.Deleted    },
      /*d*/new ct[10]{ ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Modified   , ct.Modified   , ct.Modified   , ct.Modified   , ct.Added},
      /*e*/new ct[9 ]{ ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      },
    };

    private static ct[][] expected_Diff_data2_to_data1 = new ct[5][] {
                     /* 0               1                2               3               4                5              6              7              8              9    */                      
      /*a*/new ct[9 ]{ ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  },
      /*b*/new ct[9 ]{ ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      , ct.Added      },
      /*c*/new ct[9 ]{ ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Added      , ct.Added      },
      /*d*/new ct[10]{ ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Unchanged  , ct.Modified   , ct.Modified   , ct.Modified   , ct.Modified   , ct.Deleted },
      /*e*/new ct[9 ]{ ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    , ct.Deleted    },
    };

    [Fact]
    public void Comparison() {
      string[] actual_columnOrder;
      bool equal;

      ct[][] actual_Diff_data1_to_data2 = data1.CompareData(data2, out actual_columnOrder, out equal);

      Assert.False(equal);
      Assert.Equal(expected_Diff_data1_to_data2, actual_Diff_data1_to_data2);
      Assert.Equal(expected_columnOrder, actual_columnOrder);

      ct[][] actual_Diff_data2_to_data1 = data2.CompareData(data1, out actual_columnOrder, out equal);

      Assert.False(equal);
      Assert.Equal(expected_Diff_data2_to_data1, actual_Diff_data2_to_data1);
      Assert.Equal(expected_columnOrder, actual_columnOrder);

    }

    [Fact]
    public void DetectChangeAfterPrecision() {
      Random r = new Random(32345890);
      int length = 10;
      double[] expected = new double[length];
      double[] actual = new double[length];

      for (int i = 0; i < length; i++) {
        expected[i] = r.NextDouble();
        actual[i] = expected[i] - 0.01; //loss of precision
      }

      Assert.False(expected.CompareData(actual, out ChangeType[] _, new DataDictionaryComparisonConfiguration() { DecimalPrecision = 2 }));
    }

    [Fact]
    public void CompareWithSimulatedPrecisionLoss() {
      Random r = new Random(32345890);
      int length = 10;
      double[] expected = new double[length];
      double[] actual = new double[length];

      for (int i = 0; i < length; i++) {
        expected[i] = r.NextDouble();
        actual[i] = (double)((Single)expected[i]); //loss of precision
      }

      Assert.True(expected.CompareData(actual, out ChangeType[] _, new DataDictionaryComparisonConfiguration() { DecimalPrecision = 7 }));
    }

    [Fact]
    public void CompareToSpecificPrecision() {
      Random r = new Random(32345890);
      int length = 10;
      double[] expected = new double[length];
      double[] actual = new double[length];

      for (int i = 0; i < length; i++) {
        expected[i] = r.NextDouble();
        actual[i] = expected[i] - 0.00001 ; //loss of precision
      }

      Assert.True(expected.CompareData(actual, out ChangeType[] _, new DataDictionaryComparisonConfiguration() { DecimalPrecision = 3 }));
      Assert.True(expected.CompareData(actual, out ChangeType[] _, new DataDictionaryComparisonConfiguration() { DecimalPrecision = 4 }));
      Assert.False(expected.CompareData(actual, out ChangeType[] _, new DataDictionaryComparisonConfiguration() { DecimalPrecision = 5 }));
      Assert.False(expected.CompareData(actual, out ChangeType[] _, new DataDictionaryComparisonConfiguration() { DecimalPrecision = 6 }));
    }

    [Fact]
    public void CompareDecimalNumbersToPrecision() {
      Random r = new Random(32345890);
      int length = 15;
      double[] expected = new double[length];
      double[] actual = new double[length];

      for (int i = 0; i < length; i++) {
        expected[i] = r.NextDouble() + Math.Pow(10,i);
        actual[i] = expected[i] - 0.0001; //loss of precision
      }
      Assert.True(expected.CompareData(actual, out ChangeType[] _, new DataDictionaryComparisonConfiguration() { DecimalPrecision = 3 }));
      Assert.False(expected.CompareData(actual, out ChangeType[] _, new DataDictionaryComparisonConfiguration() { DecimalPrecision = 4 }));
    }
  }
}
