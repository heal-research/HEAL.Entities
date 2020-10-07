# HEAL.Entities.Utils

# IEnumerable Extensions

## Min or Max by some Attribute
Allows querying the entry of an IEnumerable with the lowest or highes value of some property. E.g. query the person with the lowest age, or the DWH entry with the most recent load date.

```C#
public class Person {
  public string Name { get; set; }
  public int Age { get; set; }
}

public static Person p1 = new Person() { Name = "Peter", Age = 26 };
public static Person p4 = new Person() { Name = "Mary", Age = 57 };
public static Person p5 = new Person() { Name = "Herbert", Age = 45 };
public static Person[] persons = new Person[] { p1, p2, p3, p4, p5 };
```

The methods `MinOrDefaultBy` and `MaxOrDefaultBy` provide exactly this expected behavior while passing the queried IEnumerable only once.

```C#
                                          //get the person with the lowest age
var expected = persons.Where(x => x.Age == persons.Min(y => y.Age)).FirstOrDefault();
var actual = persons.MinOrDefaultBy(x => x.Age);
                                        //get the highest age     the first person with this age
var expected = persons.Where(x => x.Age == persons.Max(y => y.Age)).FirstOrDefault();
var actual = persons.MaxOrDefaultBy(x => x.Age);
```



# Compare data dictionaries
The `DataDictionaryComparison` extensions methods provide change information for a 2D, jagged data structure. This enables for example the comparison of data extracted from an excel file, with data from another source like a database and provides information about the changes, i.e. inserts, modifications, deletions. 

This could be achieved with a `Dictionary<string,IList>` where the string key stores the column name, but this would loose information about the type of data stored in the `IList`. The `DataDictionaryColumn` type stores both type of data and column name.

```C#
public class DataDictionaryColumn {
  private DataDictionaryColumn() { }

  public DataDictionaryColumn(string columnName, Type dataType) {
    this.Name = columnName;
    this.Type = dataType;
  }

  public string Name { get; private set; }
  public Type Type { get; private set; } 

  public static DataDictionaryColumn CreateColumn<T>(string columnName){
    var column = new DataDictionaryColumn();
    column.Type = typeof(T);
    column.Name = columnName;
    return column;
  }

}
```

`HEAL.Entities.Utils` provides extension methods for the type `IDictionary<DataDictionaryColumn,IList>` like for example:
```C#
...
//returns true if data is the same
public static bool CompareData(this IDictionary<DataDictionaryColumn, IList> expected,
                                    IDictionary<DataDictionaryColumn, IList> actual,
                                    DataDictionaryComparisonConfiguration config = default)
//returns true if data is the same and provides a diff for when it is not
public static bool CompareData(this IDictionary<DataDictionaryColumn, IList> expected,
                                  IDictionary<DataDictionaryColumn, IList> actual,
                                  out string[] columnOrder,
                                  out ChangeType[][] changeMatrix,
                                  DataDictionaryComparisonConfiguration config = default)
...
```
That provide diff information for column based information to create change information like in the following visual example:

| A | B | C |   | A | B | C | D |  | A | B | C | D |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | a | f |   | 1 | a |   | j |  | unchanged | unchanged |<span style="color:red"> removed</span>|<span style="color:green"> added</span>|
| 2 | b | g | **vs**| 2 | b |   | k | **results in** | unchanged | unchanged | <span style="color:red"> removed </span>|<span style="color:green"> added</span>|
| 3 | c | h |   | 3 | b |   | l |  | unchanged |<span style="color:orange"> modified  </span>| <span style="color:red"> removed </span>|<span style="color:green"> added</span>|
| 4 | d | i |   |   |   |   |  |  | <span style="color:red"> removed </span>| <span style="color:red"> removed </span>| <span style="color:red"> removed </span>|unmodified|




