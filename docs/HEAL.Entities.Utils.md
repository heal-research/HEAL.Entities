# HEAL.Entities.Utils

# IEnumerable Extensions

## Minimum or maximum by an attribute
Allows querying the entry of an IEnumerable with the lowest or highest value of a property. For example it allows to query the person with the lowest age, or the DWH entry with the most recent load date.

```C#
public class Person {
  public string Name { get; set; }
  public int Age { get; set; }
}

public static Person p1 = new Person() { Name = "Peter", Age = 26 };
public static Person p2 = new Person() { Name = "Mary", Age = 57 };
public static Person p3 = new Person() { Name = "Herbert", Age = 45 };
public static Person[] persons = new Person[] { p1, p2, p3 };
```

The methods `MinOrDefaultBy` and `MaxOrDefaultBy` provide exactly this expected behavior while passing the queried IEnumerable only once.

```C#
// find the person with the lowest age
var expected = persons.Where(x => x.Age == persons.Min(y => y.Age)).FirstOrDefault();
var actual = persons.MinOrDefaultBy(x => x.Age);

// find the person with the highest age
var expected = persons.Where(x => x.Age == persons.Max(y => y.Age)).FirstOrDefault();
var actual = persons.MaxOrDefaultBy(x => x.Age);
```



# Compare data dictionaries
The `DataDictionaryComparison` extensions methods provide change information for a 2D jagged data structure. This enables for example the comparison of data extracted from an Excel file with data from another source and provides information about the changes, i.e. inserts, modifications, deletions. 

The `DataDictionaryColumn` type stores both type of data and column name. 
```C#
public class DataDictionaryColumn {
  private DataDictionaryColumn() { }

  public DataDictionaryColumn(string name, Type dataType) {
    this.Name = name;
    this.Type = dataType;
  }

  public string Name { get; private set; }
  public Type Type { get; private set; } 

  public static DataDictionaryColumn CreateColumn<T>(string name){
    var column = new DataDictionaryColumn();
    column.Type = typeof(T);
    column.Name = name;
    return column;
  }
}
```

`HEAL.Entities.Utils` provides extension methods for the type `IDictionary<DataDictionaryColumn,IList>` like for example:
```C#
...
//returns true if data are the same
public static bool CompareData(this IDictionary<DataDictionaryColumn, IList> expected,
                                    IDictionary<DataDictionaryColumn, IList> actual,
                                    DataDictionaryComparisonConfiguration config = default)

// returns true if data are the same or provides a list of differences 
public static bool CompareData(this IDictionary<DataDictionaryColumn, IList> expected,
                                    IDictionary<DataDictionaryColumn, IList> actual,
                                    out string[] columnOrder,
                                    out ChangeType[][] differences,
                                    DataDictionaryComparisonConfiguration config = default)
...
```

The following example demonstrates the functionality of `CompareData`:
| A | B | C |   | A | B | C | D |  | A | B | C | D |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | a | f |   | 1 | a |   | j |  | unchanged | unchanged |<span style="color:red"> removed</span>|<span style="color:green"> added</span>|
| 2 | b | g | **vs**| 2 | b |   | k | **results in** | unchanged | unchanged | <span style="color:red"> removed </span>|<span style="color:green"> added</span>|
| 3 | c | h |   | 3 | b |   | l |  | unchanged |<span style="color:orange"> modified  </span>| <span style="color:red"> removed </span>|<span style="color:green"> added</span>|
| 4 | d | i |   |   |   |   |  |  | <span style="color:red"> removed </span>| <span style="color:red"> removed </span>| <span style="color:red"> removed </span>|unmodified|




