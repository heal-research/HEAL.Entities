[![Build Status](https://dev.azure.com/heal-research/HEAL.Entities/_apis/build/status/heal-research.HEAL.Entities?branchName=master)](https://dev.azure.com/heal-research/HEAL.Entities/_build/latest?definitionId=1&branchName=master)

# HEAL.Entities
HEAL.Entities provides default classes and implementations for domain driven software development and application design. Including repository implementations for:
- reading excel files
- CRUD access to any RDBMS compatible with EntityFrameworkCore.
- the DataVault-V2 data warehousing modeling schema.

HEAL.Entities.Utils provides additional handy utilities, snippets and extension methods for base c# functionality.  

## Content
1. [Getting Started](#getting-started)

1. [Features and Usage ](#features-and-usage)

1. [License](#license)

# Getting Started

## Get the Nuget Packages
All release packages of this solution can be found on the public nuget.org feed.

Additionally, we provide a public nuget build feed where you can get the latest release candidates or feature builds from our CI platform see the [development instructions](docs/development.md).

To use the packages, include the following feed URL in visual studio 
```
https://pkgs.dev.azure.com/heal-research/HEAL.Entities/_packaging/HEAL.Entities/nuget/v3/index.json
```
or adda nuget.config file to your project, in the same folder as your .csproj or .sln file with the following content
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="HEAL.Entities" value="https://pkgs.dev.azure.com/heal-research/HEAL.Entities/_packaging/HEAL.Entities/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

## Build locally or contribute

Prerequisites and build instructions can be found in the [development instructions](docs/development.md).

# Features and Usage 

- [**HEAL.Entities.DataAccess.Abstractions**](#abstract-repository)
  
  Interfaces and abstract base implementations for DataAccess

- [**HEAL.Entities.DataAccess.EFCore**](#rdbms-repositories) or visit [here](docs/HEAL.Entities.DataAccess.EFCore.md) for details

  Repository implementations for RDBMS access. Provides default implementations for abstract base classes and interfaces for domain oriented CRUD access to relational databases. Provides repositories for the data warehousing modeling schema DataVault V2. 

- [**HEAL.Entities.DataAccess.EPPlus**](#excel-repositories) or visit [here](docs/HEAL.Entities.DataAccess.EPPlus.md) for details

  Domain repository based access to Excel files. Utilizes the EPPlus library to allow for formula calculation prior to data reads. *Supports only read access as of now.*

- [**HEAL.Entities.Objects**](#domain-objects)

  Interfaces, abstract implementations or Enums for business entities as POCOs that 
  are required for usage of the access libraries.

- [**HEAL.Entities.Utils**](#utils-library) or visit [here](docs/HEAL.Entities.Utils.md) for details

  Small code snippets or extension methods that are shared by internal libraries or are  reoccurring in application projects. The contents of this project are restricted to target standard .NET namespaces. No further/external dependencies are welcome in this project in order to ensure that the Utils project can be consumed without dependency issues.

# Abstract Repository
The `HEAL.Entities.DataAccess.Abstractions` library contains the interfaces and abstract implementations of domain driven data access repositories. The base `IReadRepository<TEntity, TKey>` interface defines the bare minimum read ability and it's derived `ICRUDDomainRepository<TEntity, TKey>` defines the bare minimum of create, read, update and delete capabilities. 

```C#
public interface IReadRepository<TEntity, TKey>
    where TEntity : IDomainObject<TKey>
    where TKey : IComparable<TKey> {
  
  IEnumerable<TEntity> GetAll();

  IEnumerable<TEntity> Get( Expression<Func<TEntity, bool>> filter, 
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, 
      string includeProperties);
  
  TEntity GetByKey(TKey id);
}
```

This shared abstraction library ensures that a repositories `IPersonRepository: IReadRepository<Person,long>` implementation can switch between e.g. Excel and RDBMS implementation. This allows for example to read data from an Excel source but to store it to a Database simply by switching the implementation at runtime.

This genericity enables clear separation of concerns and easy maintenance of code, though at the higher cost of solution design and initial implementation effort.

# RDBMS Repositories
**Visit [here](docs/HEAL.Entities.DataAccess.EFCore.md) for more details.**

The RDBMS repository project provides Domain Object oriented database access for relational database management systems by utilizing the EntityFramework Core library. 

```C#
public class Student : IDomainObject<long?> 
{
  public long? StudientId { get; set; }
  public string Name { get; set; }
  public string EMail { get; set; }
}
...
using (var context = new DbContext()) {
  var repo = new CRUDRepository<Student,long>(context);
  var student = new Student(){
    Name = "Quentin Tarantino",
    EMail = "quentin@tarantino.abcde"
  }
  //.e.g. BIGINT Identity for student id
  var studentId = repo.Insert(student); 
  ...
  IEnumerable<Person> persons = repo.GetAll();
}
```

Additionally, this package also provides default repository implementations for the DWH modeling schema **DataVault-V2** and all its associated table type, Hub, Link, Satellite. Visit [here](docs/HEAL.Entities.DataAccess.EFCore.DWH.DV2.md) for more details.

# Excel Repositories
**Visit [here](docs/HEAL.Entities.DataAccess.EPPlus.md) for more details.**

The `HEAL.Entities.DataAccess.EPPlus` library is an attempt to provide easy parsing of line oriented, structure excel data as .net objects utilizing the EPPlus Library. The goal is to make the repository easy to use, with as little configuration as possible.

| Row Nr. | A              | B         |
|---------|----------------|-----------|
|         | **First Name** | **Age**   |
| 1       | Henry          | 23        |
| 2       | Penelope       | 41        |
| 3       | Julia          | 37        |

Such an example excel table can be parsed with the minimal code example shown below.

```C#
public class ExcelDataObject : IDomainObject<int>, ITestDomainObject {
  [ExcelAudit(ExcelAuditProperties.RowId)]
  public int PrimaryKey { get; set; }  
  [ExcelColumnConfiguration(ExcelColumnEnum.A)]
  public string Prename { get; set; }
  [ExcelColumnConfiguration(ExcelColumnEnum.B)]
  public int Age { get; set; }
}
...
using (var context = new ExcelContext()){
  //extracts mapping information from the attributes
  context.BuildAttributedEntity<ExcelDataObject>();

  using (var fileOptions = new ExcelFileOptions(new FileStream(@"Data\TestData.xlsx", FileMode.Open)))
  using (var repo new EPPlusDomainRepository<ExcelDataObject, int>(context,fileOptions)) {
    IEnumerable<ExcelDataObject> data = repo.GetAll();
  }
}
```

But the excel repository also supports clean separation of concerns in the plain old clr object (POCO) by providing a fluent API for property mapping comparable to .NET's Entity Framework. 

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
    ...
  }
}
...      
```

# Domain Objects
The `HEAL.Entities.Objects` project serves as a base for all domain object oriented libraries of this solution. A domain object is kept as simple as possible, and considers only comparability and unique identification as core requirements for one object. 

```C#
public interface IDomainObject<T> : IComparable<IDomainObject<T>>
                  where T : IComparable<T>
{
  T PrimaryKey { get; set; }
}     
```
This approach also enables composite or complex key support for any of the supported repositories.

# Utils Library
HEAL.Entities.Utils contains handy utility methods used in other projects of this solution that are general enough to allow and useful enough to warrant extraction into a separate project. 

The intention of this project is that it is lightweight and introduces no additional external dependencies if used. This allows HEAL.Entities.Utils to be included in any target project where any of the code might prove useful.

Snippets/Functionality provides by this project includes for example:
- data diff comparison for 2D jagged data structures
- reflection helpers for property selectors
- extensions to `IEnumerable<T>` that provide additional types of sorting variants

**Visit [here](docs/HEAL.Entities.Utils.md) for more details.**


# License 
HEAL.Entities is [licensed](LICENSE.txt) under the MIT License.
```
MIT License

Copyright (c) 2017-present Heuristic and Evolutionary Algorithms Laboratory

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
