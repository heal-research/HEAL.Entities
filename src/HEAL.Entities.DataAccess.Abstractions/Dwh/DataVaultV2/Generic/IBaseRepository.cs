using HEAL.Entities.DataAccess.Abstractions;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions {
  public interface IBaseRepository<TEntity, TPrimaryKey, THashes,TReference> : IReadRepository<TEntity, TPrimaryKey>
      where TEntity : IDataVaultObject<TPrimaryKey>
      where THashes : IComparable<THashes>
      where TPrimaryKey : IComparable<TPrimaryKey> {

    /// <summary>
    /// returns only valid entries from the DataVault.
    /// excludes link entries with END DATE or OLD satellite states. 
    /// no filteres are imposed on hub entries 
    /// </summary>
    /// <param name="filter">predicate specifying the relevant entities</param>
    /// <param name="orderBy">order clause</param>
    /// <param name="includeProperties">list of properties to be included in query</param>
    /// <returns></returns>
    IEnumerable<TEntity> GetCurrent(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");

    /// <summary>
    /// calculates and sets the <typeparamref name="TPrimaryKey"/> primary key of all entities
    /// </summary>
    /// <param name="entities"></param>
    IEnumerable<TEntity> CalculateHashes(IEnumerable<TEntity> entities);
    /// <summary>
    /// calculates the <typeparamref name="TPrimaryKey"/> for the given entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    TEntity CalculateHashes(TEntity entity);
    /// <summary>
    /// creats a new database transaction and calls the given function inside this transactions
    /// commit is called after function is executed successfully. otherwise the transaction is rolled back.
    /// </summary>
    /// <typeparam name="T">return type of the function</typeparam>
    /// <param name="function">function to be executed inside a db transaction</param>
    /// <returns>function result</returns>
    T ExecuteInTransaction<T>(Func<T> function);
    /// <summary>
    /// creats a new database transaction and calls the given action inside this transactions
    /// commit is called after action is executed successfully. otherwise the transaction is rolled back.
    /// </summary>
    /// <param name="action">action to be executed inside a db transaction</param>
    void ExecuteInTransaction(Action action);

    /// <summary>
    /// create a new DV entry
    /// </summary>
    /// <param name="entity">the entity to be inserted</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    TEntity Insert(TEntity entity, DateTime loadDate);


    /// <summary>
    /// create a new DV entry
    /// </summary>
    /// <param name="entity">the entity to be inserted</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadInformation">if entity implements <see cref="IStoreLoadInformation"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    TEntity Insert(TEntity entity, DateTime loadDate
                                  , IStoreLoadInformation loadInformation );

    /// <summary>
    /// create a new DV entry
    /// </summary>
    /// <param name="entity">the entity to be inserted</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadReference">if entity implements <see cref="IReferenceLoadInformation<THashes>"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    TEntity Insert(TEntity entity, DateTime loadDate
                                  , IReferenceLoadInformation<TReference> loadReference );

    /// <summary>
    /// create a new DV entry
    /// </summary>
    /// <param name="entity">the entity to be inserted</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadInformation">if entity implements <see cref="IStoreLoadInformation"/> you can provide this information</param>
    /// <param name="editedByInformation">if entity implements <see cref="IEditedByInformation"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    TEntity Insert(TEntity entity, DateTime loadDate
                                  , IStoreLoadInformation loadInformation 
                                  , IEditedByInformation editedByInformation );

    /// <summary>
    /// create a new DV entry
    /// </summary>
    /// <param name="entity">the entity to be inserted</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadReference">if entity implements <see cref="IReferenceLoadInformation<THashes>"/> you can provide this information</param>
    /// <param name="editedByInformation">if entity implements <see cref="IEditedByInformation"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    TEntity Insert(TEntity entity, DateTime loadDate
                                  , IReferenceLoadInformation<TReference> loadReference 
                                  , IEditedByInformation editedByInformation );

    /// <summary>
    /// create a new DV entry
    /// </summary>
    /// <param name="entity">the entity to be inserted</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadInformation">if entity implements <see cref="IStoreLoadInformation"/> you can provide this information</param>
    /// <param name="loadReference">if entity implements <see cref="IReferenceLoadInformation<THashes>"/> you can provide this information</param>
    /// <param name="editedByInformation">if entity implements <see cref="IEditedByInformation"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    TEntity Insert(TEntity entity, DateTime loadDate
                                  , IStoreLoadInformation loadInformation 
                                  , IReferenceLoadInformation<TReference> loadReference 
                                  , IEditedByInformation editedByInformation );

    /// <summary>
    /// calls <see cref="Insert(TEntity, DateTime, string)"/> for every entry
    /// </summary>
    /// <param name="entities">list of entries</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate);

    /// <summary>
    /// calls <see cref="Insert(TEntity, DateTime, string)"/> for every entry
    /// </summary>
    /// <param name="entities">list of entries</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadReference">if entity implements <see cref="IReferenceLoadInformation<THashes>"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate
                                  , IReferenceLoadInformation<TReference> loadReference );

    /// <summary>
    /// calls <see cref="Insert(TEntity, DateTime, string)"/> for every entry
    /// </summary>
    /// <param name="entities">list of entries</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadInformation">if entity implements <see cref="IStoreLoadInformation"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate
                                  , IStoreLoadInformation loadInformation );

    /// <summary>
    /// calls <see cref="Insert(TEntity, DateTime, string)"/> for every entry
    /// </summary>
    /// <param name="entities">list of entries</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadReference">if entity implements <see cref="IReferenceLoadInformation<THashes>"/> you can provide this information</param>
    /// <param name="editedByInformation">if entity implements <see cref="IEditedByInformation"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate
                                  , IReferenceLoadInformation<TReference> loadReference 
                                  , IEditedByInformation editedByInformation );

    /// <summary>
    /// calls <see cref="Insert(TEntity, DateTime, string)"/> for every entry
    /// </summary>
    /// <param name="entities">list of entries</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadInformation">if entity implements <see cref="IStoreLoadInformation"/> you can provide this information</param>
    /// <param name="editedByInformation">if entity implements <see cref="IEditedByInformation"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate
                                  , IStoreLoadInformation loadInformation 
                                  , IEditedByInformation editedByInformation );

    /// <summary>
    /// calls <see cref="Insert(TEntity, DateTime, string)"/> for every entry
    /// </summary>
    /// <param name="entities">list of entries</param>
    /// <param name="loadDate">use the timestamp of application or process start instead of a new timestamp for every insert</param>
    /// <param name="loadInformation">if entity implements <see cref="IStoreLoadInformation"/> you can provide this information</param>
    /// <param name="loadReference">if entity implements <see cref="IReferenceLoadInformation<THashes>"/> you can provide this information</param>
    /// <param name="editedByInformation">if entity implements <see cref="IEditedByInformation"/> you can provide this information</param>
    /// <returns>same entities with a set primary key and set audit attributes</returns>
    List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate
                                  , IStoreLoadInformation loadInformation 
                                  , IReferenceLoadInformation<TReference> loadReference 
                                  , IEditedByInformation editedByInformation );
  }
}