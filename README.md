[![Build Status](https://dev.azure.com/heal-research/HEAL.Entities/_apis/build/status/heal-research.HEAL.Entities?branchName=master)](https://dev.azure.com/heal-research/HEAL.Entities/_build/latest?definitionId=1&branchName=master)

# HEAL.Entities
HEAL.Entities provides default classes and implementations for domain driven software development and application design. Including repository implementations for:
- CRUD access to any RDBMS compatible with EntityFrameworkCore,
- the Data Vault 2.0 data model,
- reading Excel files

HEAL.Entities.Utils provides additional utilities, snippets, and extension methods.

## Content
1. [Getting Started](#getting-started)

1. [Features and Usage](#features-and-usage)

1. [License](#license)

# Getting Started

## Get the Nuget Packages
All release packages of this solution can be found on the public nuget.org feed.

Additionally, we provide a public nuget build feed where you can get the latest release candidate or feature builds from our CI platform see the [development instructions](docs/development.md).

To use the package include the following feed URL in visual studio 
```
https://pkgs.dev.azure.com/heal-research/HEAL.Entities/_packaging/HEAL.Entities/nuget/v3/index.json
```
or add a nuget.config file to your project, in the same folder as your .csproj or .sln file with the following content
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="HEAL.Entities" value="https://pkgs.dev.azure.com/heal-research/HEAL.Entities/_packaging/HEAL.Entities/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

## Build Locally or Contribute

Prerequisites and build instructions can be found in the [development instructions](docs/development.md).

# Features and Usage 

- [**HEAL.Entities.DataAccess.Abstractions**](#abstract-repository)

  Interfaces and abstract base implementations for DataAccess

- [**HEAL.Entities.DataAccess.EFCore**](#rdbms-repositories) or visit [here](docs/HEAL.Entities.DataAccess.EFCore.md) for details

  Repository implementations for RDBMS access. Provides default implementations for abstract base classes and interfaces for domain oriented CRUD access to relational databases. Provides repositories for Data Vault 2.0.

- [**HEAL.Entities.DataAccess.EPPlus**](#excel-repositories) or visit [here](docs/HEAL.Entities.DataAccess.EPPlus.md) for details

  Domain repository based access to Excel files. Utilizes [EPPlus](https://github.com/EPPlusSoftware/EPPlus) for formula calculation prior to data reads. *Currently only supports only read access.*

- [**HEAL.Entities.Objects**](#domain-objects)
  Interfaces, abstract implementations and enumerated types for business entities impllemented as POCOs that 
  are required for the data access libraries.

- [**HEAL.Entities.Utils**](#utils-library) or visit [here](docs/HEAL.Entities.Utils.md) for details

  Code snippets and extension methods that are shared by internal libraries or are reoccurring in multiple projects. The contents of this project are restricted to target standard .NET namespaces. No further/external dependencies are welcome in this project in order to ensure that the Utils project can be consumed without dependency issues.

# Abstract Repository
The `HEAL.Entities.DataAccess.Abstractions` library contains the interfaces and abstract implementations of domain driven data access repositories. The `IReadRepository<TEntity, TKey>` interface shown below defines the minimum methods required for read-only access. The `ICRUDDomainRepository<TEntity, TKey>` interface specializes functionality for create, read, update and delete capabilities. 

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

This shared abstraction library ensures that repository can switch between e.g. Excel and RDBMS implementation (e.g. a repository `IPersonRepository: IReadRepository<Person,long>`). This allows for example to read data from an Excel source but to store it to a Database simply by switching the implementation at runtime.
This enables clear separation of concerns and easy maintenance of code, though at the higher cost of solution design and initial implementation effort.

# RDBMS Repositories
**Visit [HEAL.Entities.DataAccess.EFCore documentation](docs/HEAL.Entities.DataAccess.EFCore.md) for more details.**

The RDBMS repository project provides domain oriented database access by utilizing the EntityFramework Core library. 

Usage example:
```C#
public class Student : IDomainObject<long?> 
{
  public long? StudentId { get; set; }
  public string Name { get; set; }
  public string EMail { get; set; }
}
...
using (var context = new DbContext()) {
  var repo = new CRUDRepository<Student,long>(context);
  var student = new Student(){
    Name = "Xenocrates",
    EMail = "Xenocrates@mail.bc"
  }

  // e.g. BIGINT Identity for student id
  var studentId = repo.Insert(student); 
  ...
  IEnumerable<Person> persons = repo.GetAll();
}
```

This package also provides default repository implementations for the **Data Vault 2.0** schema with Hub, Link, and Satellite tables. Visit [here](docs/HEAL.Entities.DataAccess.EFCore.DWH.DV2.md) for more details.

# Excel Repositories
**Visit [here](docs/HEAL.Entities.DataAccess.EPPlus.md) for more details.**

`HEAL.Entities.DataAccess.EPPlus` allows access to work with line-oriented Excel data sheets through .NET objects and utilizes [EPPlus](https://github.com/EPPlusSoftware/EPPlus).

Example for data stored in an Excel sheet:
| Row Nr. | A              | B         |
|---------|----------------|-----------|
|         | **First Name** | **Age**   |
| 1       | Henry          | 23        |
| 2       | Penelope       | 41        |
| 3       | Julia          | 37        |

This table can be read with the following code.
```C#
public class Person : IDomainObject<int>, ITestDomainObject {
  [ExcelAudit(ExcelAuditProperties.RowId)]
  public int PrimaryKey { get; set; }  
  [ExcelColumnConfiguration(ExcelColumnEnum.A)]
  public string Name { get; set; }
  [ExcelColumnConfiguration(ExcelColumnEnum.B)]
  public int Age { get; set; }
}
...
using (var context = new ExcelContext()) {
  // extracts mapping information from the attributes
  context.BuildAttributedEntity<Person>();

  using (var fileOptions = new ExcelFileOptions(new FileStream(@"Data\TestData.xlsx", FileMode.Open)))
  using (var repo = new EPPlusDomainRepository<Person, int>(context, fileOptions)) {
    IEnumerable<Person> data = repo.GetAll();
  }
}
```

The Excel repository supports clean separation of concerns in the POCO by providing a fluent API for property mapping similar to .NET Entity Framework. 

```C#
public class TestExcelContext : ExcelContext {

  public TestExcelContext(ExcelOptions options) : base(options) { }

  protected override void OnCreating(ExcelModelBuilder modelBuilder) {
    modelBuilder.Entity<Person>(Configure_ExcelDomainObjectConfiguration);
  }

  private void Configure_ExcelDomainObjectConfiguration(ExcelEntityBuilder<Person> builder) {
    builder.AuditRowId(x => x.PrimaryKey);

    builder.Property(x => x.Name)
      .Column(ExcelColumnEnum.A)
      .HasHeaderName("First Name");
    ...
  }
}
...      
```

# Domain Objects
`HEAL.Entities.Objects` serves as a base for all domain object oriented libraries of this solution. Domain objects have a unique identification for comparison of objects.

```C#
public interface IDomainObject<T> : IComparable<IDomainObject<T>>
                  where T : IComparable<T>
{
  T PrimaryKey { get; set; }
}     
```
Composite keys for repositories are supported as well.

# Utils Library
HEAL.Entities.Utils contains utility methods used in multiple projects of this solution that are general enough to warrant extraction into a separate project. 

This project is lightweight and introduces no additional external dependencies. 

Functionality includes for example:
- Algorithm for finding data differences between 2D jagged data structures
- reflection helpers for property selectors
- extension methods for `IEnumerable<T>` that provide sorting functionality

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

# Closing Remarks
Excel is a registered trademark of Microsoft.