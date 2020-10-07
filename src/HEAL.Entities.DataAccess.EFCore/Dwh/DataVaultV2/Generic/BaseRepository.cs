using HEAL.Entities.Objects;
using HEAL.Entities.DataAccess.Abstractions;
using HEAL.Entities.DataAccess.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using HEAL.Entities.Objects.Dwh.DataVaultV2;
using HEAL.Entities.DataAccess.Dwh.DataVaultV2.Generic.Abstractions;

namespace HEAL.Entities.DataAccess.EFCore.Dwh.DataVaultV2.Generic {


  /// <summary>
  /// derive from this class to inherit the base implementation for repository data-access to a DWH in DataVault 2.0 Schema
  /// </summary>
  /// <typeparam name="TEntity">Generic type of the managed domain entity</typeparam>
  /// <typeparam name="TPrimaryKey">type of the primary key</typeparam>
  public abstract class BaseRepository<TEntity, TPrimaryKey, THashes, TReference> : IBaseRepository<TEntity, TPrimaryKey, THashes,TReference>, IDisposable
      where TEntity : class, IDataVaultObject<TPrimaryKey>
      where THashes : IComparable<THashes>
      where TPrimaryKey : IComparable<TPrimaryKey> {
    protected ILogger<BaseRepository<TEntity, TPrimaryKey, THashes, TReference>> Logger { get; set; }

    /// <summary>
    /// internal context of the generic EFDataVaultRepository
    /// Use this property for manual Queries needed in the specific Repositories
    /// </summary>
    protected DwhDbContext Context { get; }

    /// <summary>
    /// internal DbSet of the generic EFDataVaultRepository
    /// Use this property for manual Queries needed in the specific DomainObject
    /// </summary>
    protected DbSet<TEntity> DbSet { get; }

    protected DataVaultHashFunction<THashes> HashFunction { get; }

    public BaseRepository(DwhDbContext context, DataVaultHashFunction<THashes> hashFunction, ILogger<BaseRepository<TEntity, TPrimaryKey, THashes, TReference>> logger) {
      this.Context = context;
      this.DbSet = context.Set<TEntity>();
      if (hashFunction == null)
        throw new ArgumentNullException("HashFunction", "'HashFunction' was not supplied. an implementation of DataVaultHashFunction<TPKey> has to be supplied.");
      this.HashFunction = hashFunction;
      this.Logger = logger;
    }

    public virtual IEnumerable<TEntity> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "") {
      IQueryable<TEntity> query = DbSet;

      if (filter != null) {
        query = query.Where(filter);
      }

      foreach (var includeProperty in includeProperties.Split
          (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
        query = query.Include(includeProperty);
      }

      if (orderBy != null) {
        return orderBy(query).ToList();
      } else {
        return query.ToList();
      }
    }

    public virtual TEntity GetByKey(TPrimaryKey id) {
      return DbSet.Find(id);
    }




    protected virtual TEntity AddEntity(TEntity entity, DateTime loadDate
                                  , IStoreLoadInformation loadInformation
                                  , IReferenceLoadInformation<TReference> loadReference
                                  , IEditedByInformation editedByInformation) {
      entity.LoadDate = loadDate;

      SetLoadReference(entity as IReferenceLoadInformation<TReference>, loadReference);
      SetStoreLoadInformation(entity as IStoreLoadInformation, loadInformation);
      SetEditedBy(entity as IEditedByInformation, editedByInformation);

      CalculateHashes(entity);
      DBSetAddEntry(entity);

      return entity;
    }

    protected virtual void SetEditedBy(IEditedByInformation edited, IEditedByInformation editedByInformation) {
      if (edited != null) {
        edited.EditedBy = editedByInformation.EditedBy;
      }
    }

    protected virtual void SetStoreLoadInformation(IStoreLoadInformation storeLoad, IStoreLoadInformation loadInformation) {
      if (storeLoad != null && loadInformation != null) {
        storeLoad.RecordSource = loadInformation.RecordSource;
        storeLoad.LoadedBy = loadInformation.LoadedBy;
      }
    }

    protected virtual void SetLoadReference(IReferenceLoadInformation<TReference> referenceLoad, IReferenceLoadInformation<TReference> loadReference) {
      if (referenceLoad != null && loadReference != null) {
        referenceLoad.LoadReference = loadReference.LoadReference;
      }
    }

    public abstract TEntity CalculateHashes(TEntity entity);
    protected abstract void DBSetAddEntry(TEntity entity);

    public IEnumerable<TEntity> CalculateHashes(IEnumerable<TEntity> entities) {
      foreach (var entity in entities) {
        yield return CalculateHashes(entity);
      }
    }

    public virtual IEnumerable<TEntity> GetCurrent(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "") {
      return Get(filter, orderBy, includeProperties);
    }

    public T ExecuteInTransaction<T>(Func<T> func) {
      using (var transaction = Context.Database.BeginTransaction()) {
        var funcResult = func();
        transaction.Commit();
        return funcResult;
      }
    }

    public void ExecuteInTransaction(Action func) {
      using (var transaction = Context.Database.BeginTransaction()) {
        func();
        transaction.Commit();
      }
    }

    public virtual void Dispose() {
    }

    public long Count(Expression<Func<TEntity, bool>> filter = null) {
      IQueryable<TEntity> query = DbSet;
      if (filter != null) {
        query = query.Where(filter);
      }
      return query.LongCount();
    }

    public IEnumerable<TEntity> GetAll() {
      IQueryable<TEntity> query = DbSet;
      return query.ToList();
    }

    public virtual TEntity Insert(TEntity entity, DateTime loadDate
                                  , IStoreLoadInformation loadInformation
                                  , IReferenceLoadInformation<TReference> loadReference
                                  , IEditedByInformation editedByInformation ) {
      AddEntity(entity, loadDate, loadInformation, loadReference, editedByInformation);
      Context.SaveChanges();
      return entity;
    }

    public virtual List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate
                                  , IStoreLoadInformation loadInformation
                                  , IReferenceLoadInformation<TReference> loadReference 
                                  , IEditedByInformation editedByInformation) {
      foreach (var entity in entities) {
        AddEntity(entity, loadDate, loadInformation, loadReference, editedByInformation);
      }
      Context.SaveChanges();
      return entities.ToList();
    }

    public TEntity Insert(TEntity entity, DateTime loadDate) {
      return Insert(entity, loadDate, null, null, null);
    }

    public TEntity Insert(TEntity entity, DateTime loadDate, IStoreLoadInformation loadInformation ) {
      return Insert(entity, loadDate, loadInformation, null, null);
    }

    public TEntity Insert(TEntity entity, DateTime loadDate, IReferenceLoadInformation<TReference> loadReference ) {
      return Insert(entity, loadDate, null, loadReference, null);
    }

    public TEntity Insert(TEntity entity, DateTime loadDate, IStoreLoadInformation loadInformation , IEditedByInformation editedByInformation ) {
      return Insert(entity, loadDate, loadInformation, null, editedByInformation);
    }

    public TEntity Insert(TEntity entity, DateTime loadDate, IReferenceLoadInformation<TReference> loadReference , IEditedByInformation editedByInformation ) {
      return Insert(entity, loadDate, null, loadReference, editedByInformation);
    }

    public List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate) {
      return Insert(entities, loadDate, null, null, null);
    }

    public List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate, IReferenceLoadInformation<TReference> loadReference ) {
      return Insert(entities, loadDate, null, loadReference, null);
    }

    public List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate, IStoreLoadInformation loadInformation ) {
      return Insert(entities, loadDate, loadInformation, null, null);
    }

    public List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate, IReferenceLoadInformation<TReference> loadReference, IEditedByInformation editedByInformation) {
      return Insert(entities, loadDate, null, loadReference, editedByInformation);
    }

    public List<TEntity> Insert(IEnumerable<TEntity> entities, DateTime loadDate, IStoreLoadInformation loadInformation, IEditedByInformation editedByInformation) {
      return Insert(entities, loadDate, loadInformation, null, editedByInformation);
    }
  }
}
