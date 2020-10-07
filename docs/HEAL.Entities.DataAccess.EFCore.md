# HEAL.Entities.DataAccess.EFCore


# CRUD Repository

![ER Diagram](img/HEAL.Entities.DataAccess.EFCore.ERD.png)

The RDBMS repository project provides Domain Object oriented database access for relational database management systems by utilizing the EntityFramework Core library. 

```C#
//basic read functions
public interface IReadRepository<TEntity, TKey>
    where TEntity : IDomainObject<TKey>
    where TKey : IComparable<TKey> {    
  long Count(Expression<Func<TEntity, bool>> filter = null);
  IEnumerable<TEntity> GetAll();
  IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
                          Func<IQueryable<TEntity>,
                          IOrderedQueryable<TEntity>> orderBy = null,
                          string includeProperties = "");
  TEntity GetByKey(TKey id);
}
//read repository extended to CRUD (create, read, update, delete) repository
public interface ICRUDDomainRepository<TEntity, TKey> : IReadRepository<TEntity, TKey>
    where TEntity : IDomainObject<TKey>
    where TKey : IComparable<TKey> {
  TKey Insert(TEntity entity);
  void Delete(TEntity entityToDelete);
  void Delete(TKey id);
  void Update(TEntity entityToUpdate);
}
```

Usage of the Repository is as simple as create a new repository instance with the generic parameters. 

```C#
public class Student : IDomainObject<long?> 
{
  public long? StudientId { get; set; }
  public string Name { get; set; }
  public string EMail { get; set; }
}
...
using (var context = new YourDbContext()) {
  var repo = new CRUDRepository<Student,long>(context);
  var student = new Student(){
    Name = "Miley Elliott",
    EMail = "Miley@Elliott.abcde"
  }

  //.e.g. BIGINT Identity for student id
  var studentId = repo.Insert(student); 
  ...
  IEnumerable<Person> persons = repo.GetAll();
}
```