# HEAL.Entities.DataAccess.EPPlus

`HEAL.Entities.DataAccess.EPPlus` provides domain object oriented data access for Excel files and utilizes [EPPlus](https://github.com/EPPlusSoftware/EPPlus).

Assume we want to parse the following Excel table.

| A              | B             | C       | D              | E          | F                  | G                | H                             |
|----------------|---------------|---------|----------------|------------|--------------------|------------------|-------------------------------|
| **First Name** | **Last Name** | **Age** | **Occupation** | **Salary** | **Marital Status** | **Has Children** | **Hobbies**                   |
| Miley          | Elliott       | 23      | Medic          | 59.63      | Married            | 1                | Ice skating, Quizzes          |
| Penelope       | Scott         | 25      |                | 75.31      | Married            | 0                | Sculpting, Drawing, Animation |
| Henry          | Murray        | 25      | Journalist     | 62.87      | Married            | 0                | Cooking, Juggling             |

The result should be an `IEnumerable<Person>`. The Excel repository requires information about the column mapping of individual properties of the Person domain object. This mapping information contains information that e.g. Prename is called 'First Name' in the Excel and stores string values in column A. 

Such mapping information can be configured by two different methods. By [attribute based annotations](#mapping-by-attributes) added to the domain object. Or by utilizing the [fluent API](#mapping-using-the-fluentAPI) of the ExcelContext's `ModelBuilder` to map individual attributes similar to Microsoft's EntityFramework. 

An annotated DomainObject handles the mapping of Excel columns to attributes. To ensure that column headers of the Excel are checked for match of property name or provided name, a column name can be supplied as additional attribute parameter. The parser will log a warning if no match could be found.

# Mapping by Attributes
Usage of attributes for mapping of class properties to Excel columns results in less clean POCO domain object classes references to the storage technology, i.e. Excel, are present in the class definition. However it should be noted that these attributes are defined in base `HEAL.Entities.Objects` library and have to technology specific dependencies, and therefore can be considered a still 'quite' clean approach. 

However, as soon as Excel structure definition changes over different files the fluentAPI variant is the way to go. 

```C#
public class DomainObject_Attributes : IDomainObject<int>, ITestDomainObject {
  [ExcelAudit(ExcelAuditProperties.RowId)]
  public int PrimaryKey { get; set; }  
  
  [ExcelColumnConfiguration(ExcelColumnEnum.A, "First Name")]
  public string Prename { get; set; }
  [ExcelColumnConfiguration(2,"Last Name")]
  public string Surname { get; set; }
  [ExcelColumnConfiguration("C","Age")]
  public int Age { get; set; }
  [ExcelColumnConfiguration(ExcelColumnEnum.D, DefaultValue: UnitTestMetadata.DefaultOccupation)]
  public string Occupation { get; set; }
  [ExcelColumnConfiguration(ExcelColumnEnum.E)]
  public decimal Salary { get; set; }
  [ExcelColumnConfiguration(ExcelColumnEnum.F, "Marital Status", cellParser: typeof(MaritalStatusParser))]
  public bool Married { get; set; }
  [ExcelColumnConfiguration(ExcelColumnEnum.G, "Has Children", cellParser: typeof(BinaryBooleanParser))]
  public bool HasChildren { get; set; }
  [ExcelColumnConfiguration(ExcelColumnEnum.H, cellParser: typeof(CommaSeparatedListParser))]
  public string[] Hobbies { get; set; }
}
```

Since .NET restricts constructor attributes of attributes to only [attribute parameter types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/attributes#attribute-parameter-types) the custom parser implementations cannot be passed as an inline expression or a delegate. Instead the type of the parser implementing `ICellParser` is passed as parameter (See attributes Married, HasChildren, Hobbies and compare to the similar Attributes of the fluentAPI syntax).

In the attributes constructor, the passed type is instantiated using the default (empty) constructor and a handle to it's parsing method is passed along to the ExcelContext and subsequently to the repository. 

In order to create an ExcelContext for attribute based mappings follow the example of the unit tests. By creating a new base ExcelContext and calling the mapping function for each individual type that is mapped by attribute definitions.

```C#
//create a base context
var context = new ExcelContext();
//tell it to map your entity (parses the custom attributes)
context.BuildAttributedEntity<DomainObject_Attributes>();
```

# Mapping using the fluentAPI
Usage of the fluentAPI for mapping of class properties to Excel columns results in cleaner POCO domain object classes as no reference to the storage technology is present.
```c#
public class DomainObject_FluentApi : IDomainObject<int> {
  public int PrimaryKey { get; set; }

  public string Prename { get; set; }
  public string Surname { get; set; }
  public int Age { get; set; }
  public string Occupation { get; set; }
  public decimal Salary { get; set; }
  public bool Married { get; set; }
  public bool HasChildren { get; set; }
  public string[] Hobbies { get; set; }

  public int CompareTo(IDomainObject<int> other) {
    throw new NotImplementedException();
  }
}
```
Similar to the `DbContext` implementation of EntityFramework, the `ExcelContext` class calls a `virtual void OnCreating(ExcelModelBuilder modelBuilder)` method which can be overridden by the deriving specialized context. In this method the user can start the mapping and configuration of additional entities.

```C#
public class TestExcelContext : ExcelContext {

  public TestExcelContext(ExcelOptions options) : base(options) {
  }

  protected override void OnCreating(ExcelModelBuilder modelBuilder) {
    modelBuilder.Entity<DomainObject_FluentApi>(Configure_ExcelDomainObjectConfiguration);
  }

  private void Configure_ExcelDomainObjectConfiguration(ExcelEntityBuilder<DomainObject_FluentApi> builder) {
    builder.AuditRowId(x => x.PrimaryKey);

    builder.Property(x => x.Prename)
      .Column(ExcelColumnEnum.A)
      .HasHeaderName("First Name");

    builder.Property(x => x.Surname)
      .Column(2)
      .HasHeaderName("Last Name");

    builder.Property(x => x.Age)
      .Column("C");

    builder.Property(x => x.Occupation)
      .Column(ExcelColumnEnum.D)
      .WithDefault(UnitTestMetadata.DefaultOccupation);

    builder.Property(x => x.Salary)
      .Column(ExcelColumnEnum.E);

    builder.Property(x => x.Married)
      .Column(ExcelColumnEnum.F)
      .UseCustomParser((x)=> x=="Married" ? true : false);

    builder.Property(x => x.HasChildren)
      .Column(ExcelColumnEnum.G)
      .UseCustomParser(new BinaryBooleanParser());

    builder.Property(x => x.Hobbies)
      .Column(ExcelColumnEnum.H)
      .UseCustomParser((s) => string.IsNullOrEmpty(s) ? new string[0] : s.Split(',').Select(x => x.Trim()).ToArray());
  }
}
```

As the fluent API has no additional restrictions by the .NET runtime you can also defined the cell parsers as either ICellParser instances or inline Expressions. The fluentAPI is also ably to determine the target type from the property selector and therefore even the return type of the parser can be checked and determined at build time. 

# Mapping the column position
Each mapped attribute of the domain object normally corresponds to one column of the Excel. This column address can be defined by either using the string address, e.g. 'AB', of a column or its corresponding int value 28 or last but not least utilizing the `ExcelColumnEnum` enumeration that provides 250 default values and returns the corresponding int column index.

```C#
//attribute based

[ExcelColumnConfiguration(ExcelColumnEnum.D)]
public string Occupation { get; set; }
```

```C#
//fluentAPI based

builder.Property(x => x.Occupation)
  .Column(ExcelColumnEnum.D)
  .WithDefault("UNEMPLOYED");
```

# Using additional Excel attributes
Additional Excel information that might not be stored only in columns might also be interesting for data access methods.
E.g. the Excel row id might me used as INT based row ID.

```C#
//attribute based

[ExcelAudit(ExcelAuditProperties.RowId)]
public int PrimaryKey { get; set; }  
```

```C#
//fluentAPI based

builder.AuditRowId(x => x.PrimaryKey);
```

# Providing a custom parser
Boolean parsing (from a text source) works out of the box for the English "true" and "false" since those are recognized by .net as valid Boolean string values. For the bit-values "0","1" or other string representation of a boolean like "married"/"single" we need a way to specify a Parser, taking the string representation of the cell contents and returning the target value.

Any parser implementing the following interface can be used to provide this functionality.

```C#
public interface ICellParser
{
  object ParseValue(string cellValue);
}
```

A Custom cell parser could therefore also be used to parse e.g. comma-separated values or json values from a cell and provide a complex object.

```C#
public class MaritalStatusParser : ICellParser {
  public object ParseValue(string cellValue) {
    switch (cellValue) {
      case "Married": return true;
      case "Single": return false;
      default:
        throw new NotSupportedException();
    }
  }
}
public class CommaSeparatedListParser : ICellParser {
  public object ParseValue(string cellValue) {
    if (string.IsNullOrEmpty(cellValue))
      return new string[0];
    return cellValue.Split(',').Select(x => x.Trim()).ToArray();
  }
}
```
In order to add a custom parser for one attribute consider the two different syntax examples for either attribute based definition.
```c#
//for defintion at the POCO

[ExcelColumnConfiguration(ExcelColumnEnum.F, "Marital Status", cellParser: typeof(MaritalStatusParser))]
public bool Married { get; set; }
[ExcelColumnConfiguration(ExcelColumnEnum.G, "Has Children", cellParser: typeof(BinaryBooleanParser))]
public bool HasChildren { get; set; }
```
Or the fluentAPI based approach
```C#
//for definition at the model builder 

builder.Property(x => x.Married)
  .Column(ExcelColumnEnum.F)
  //inline epression
  .UseCustomParser((x)=> x=="Married" ? true : false);

builder.Property(x => x.HasChildren)
  .Column(ExcelColumnEnum.G)
  //instance implementing ICellParser 
  .UseCustomParser(new BinaryBooleanParser());
```

# Providing a default value
Default values are applied if the cell contents of the Excel column are empty, i.e. null. Instead of the .net default(<AttributeType>) the specified default value is applied to the property. For example lets assume the occupation should read 'UNEMPLOYED' if the field is left empty in the Excel, or you store some complex type that has a non trivial default value.

```C#
//attribute based

[ExcelColumnConfiguration(ExcelColumnEnum.D, DefaultValue: "UNEMPLOYED")]
public string Occupation { get; set; }
```

```C#
//fluentAPI based

builder.Property(x => x.Occupation)
  .Column(ExcelColumnEnum.D)
  .WithDefault("UNEMPLOYED");
```


# Providing a column name
Domain object property name and column headers in Excel might not match. Per default this no issue for the Excel repository, but this default behavior can be change in the `ExcelOptions` that can be passed in the constructor of an `ExcelContext`. If no `ExcelOptions` instance is passed, a default one is created.

```C#
//default
var context = new ExcelContext();
//alternative
var options = new ExcelOptions();
options.ThrowExceptionOnMismatchingColumnNames = true;
var context = new ExcelContext(options);
```

The definition of the header values differs from attribute-based definition to fluentAPI-based definition.
```C#
//attribute based
[ExcelColumnConfiguration(ExcelColumnEnum.A, "First Name")]
public string Prename { get; set; }

//fluentAPI based
builder.Property(x => x.Prename)
  .Column(ExcelColumnEnum.A)
  .HasHeaderName("First Name");
```

# Detect the end of Data
Alternatively to a fixed maximum line number, the end of data stream can also be detected by an delimiter function. This is thought to be the default usage case. Such a delimiter can be added once for every domain object that is tracked by a context. 

This works in the following manner. A row is still parsed as usual and the repository still creates the actual domain object. But, since the parsed row contains no further values in the Excel, you can take notice of this fact in some domain object attribute. The Id that tracks the Excel row number is still populated, but for example the surname and prename field are now both empty. 

Therefore we assume that the end of data in the file has been reached.

```C#
//how does the Domain object look like when no data is available anymore?
//in our case primary key will still have the row id but both name fields will be null or empty string
context.AddEndDelimiter<DomainObject_Attributes>(x => string.IsNullOrEmpty(x.Prename) && string.IsNullOrEmpty(x.Surname));
```

# ExcelFileOptions
After specifying the structure of the object that's about to be parsed in the `ExcelContext` additional information about the actual file needs to be defined in the `ExcelFileContext`. This configuration class contains the actual file location, i.e. stream, access password or for example the maximum number of rows one may parse, which line contains the header and which line number starts the data rows.

```C#
//most simple variant uses only a file stream and default values.
var fileOptions = new ExcelFileOptions(new FileStream(@"Data\TestData.xlsx", FileMode.Open))

fileOptions.DataMaximumLineNumber { get; set; } = DEFAULT_END_UNLIMITED = 0;
fileOptions.HeaderLineNumber { get; set; } = DEFAULT_HEADER_INDEX = 1;
fileOptions.DataStartLineNumber { get; set; } = DEFAULT_DATA_START_INDEX = 2;
fileOptions.WorksheetName { get; set; } = DEFAULT_WORKSHEET_NAME = "Sheet1";
fileOptions.FilePassword { get; set; } = default = null;
fileOptions.ExcelFileStream { get; set; }
```

# Repository creation
Is as simple as instantiating a new repository for the desired domain object, this object must be mapped in the `ExcelContext` that is passed in the constructer, and passing along the `ExcelFileOptions` and `ExcelContext` instance.

```c#
                         //uses default ExcelOptions
using (var context = new ExcelContext()){

  //extracts mapping information from the attributes
  context.BuildAttributedEntity<ExcelDataObject>();

  using (var fileOptions = new ExcelFileOptions(new FileStream(@"Data\TestData.xlsx", FileMode.Open)))
                      //create the repository
  using (var repo new EPPlusDomainRepository<ExcelDataObject, int>(context,fileOptions)) {
    IEnumerable<ExcelDataObject> data = repo.GetAll();
  }
}
```
